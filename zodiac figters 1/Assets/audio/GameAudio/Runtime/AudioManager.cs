using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameAudio
{
    // Central sound player. Plays SoundEvents from a fixed pool of voices, enforces each
    // event's spam limits, crossfades music, and applies the player's VolumeSettings.
    //
    // Setup:  add an AudioManager to your first scene to choose mixer groups and voice count.
    //         If there is none, one is created with default settings the first time a sound plays.
    // Usage:  hitSound.Play(transform.position);
    //         SoundHandle loop = engineSound.PlayAttached(transform);   …   loop.Stop(0.3f);
    //         AudioManager.PlayMusic(battleTheme, fadeSeconds: 2f);
    [DefaultExecutionOrder(-100)]   // Awake before gameplay scripts that may play sounds in their own Awake
    public class AudioManager : MonoBehaviour
    {
        [Header("Voices")]
        [Tooltip("How many sound effects can play at once. When all are busy, the oldest sound of equal or lower priority is cut.")]
        [SerializeField, Min(1)] int voiceCount = 32;
        [Tooltip("Keep this object alive across scene loads so sounds and music aren't cut off.")]
        [SerializeField] bool persistAcrossScenes = true;

        [Header("Mixer Routing (optional)")]
        [Tooltip("Group used by sound events that don't set their own.")]
        [SerializeField] AudioMixerGroup sfxGroup;
        [SerializeField] AudioMixerGroup musicGroup;

        [Header("Music")]
        [Tooltip("Music keeps playing while AudioListener.pause is true (e.g. in a pause menu).")]
        [SerializeField] bool musicIgnoresPause = true;

        [Header("2D Panning")]
        [Tooltip("For 2D sounds played at a position: pans them left/right by their horizontal screen position. 0 = off, 1 = full.")]
        [SerializeField, Range(0f, 1f)] float screenPan = 0f;

        class Voice
        {
            public AudioSource   source;
            public SoundEvent    sfx;          // null for music
            public VolumeChannel channel;
            public float         baseVolume;   // the sound's own volume, before fades and player settings
            public float         fade = 1f;    // 0–1, driven by fade-ins and fade-outs
            public Coroutine     fadeRoutine;
            public Transform     follow;
            public bool          isAttached;   // true while following a transform that hasn't been destroyed
            public int           generation;   // bumped on every reuse so stale SoundHandles stop matching
            public float         startTime;
        }

        class EventState
        {
            public float lastPlayTime = float.NegativeInfinity;
            public int   lastClip     = -1;
        }

        static AudioManager instance;
        static bool         quitting;

        Voice[] voices;
        Voice[] music;
        int activeMusic;
        readonly Dictionary<SoundEvent, EventState> states = new Dictionary<SoundEvent, EventState>();

        // The active manager. Created with default settings on first access if none exists.
        public static AudioManager Instance
        {
            get
            {
                if (instance == null && !quitting && Application.isPlaying)
                    new GameObject(nameof(AudioManager)).AddComponent<AudioManager>();
                return instance;
            }
        }

        // ── Lifecycle ───────────────────────────────────────────────────────────────

        // Clears statics when domain reload is disabled in Enter Play Mode settings.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() { instance = null; quitting = false; }

        void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            if (persistAcrossScenes) { transform.SetParent(null); DontDestroyOnLoad(gameObject); }

            voices = new Voice[voiceCount];
            for (int i = 0; i < voiceCount; i++)
                voices[i] = new Voice { source = CreateSource("Voice " + i) };

            music = new Voice[2];
            for (int i = 0; i < 2; i++)
            {
                var source = CreateSource(i == 0 ? "Music A" : "Music B");
                source.outputAudioMixerGroup = musicGroup;
                source.ignoreListenerPause   = musicIgnoresPause;
                source.priority              = 0;
                music[i] = new Voice { source = source, channel = VolumeChannel.Music };
            }

            VolumeSettings.Changed += OnVolumeChanged;
        }

        AudioSource CreateSource(string sourceName)
        {
            var go = new GameObject(sourceName);
            go.transform.SetParent(transform, false);
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake  = false;
            source.spatialBlend = 0f;
            return source;
        }

        void OnApplicationQuit() => quitting = true;

        void OnDestroy()
        {
            if (instance != this) return;
            instance = null;
            VolumeSettings.Changed -= OnVolumeChanged;
        }

        void LateUpdate()
        {
            if (voices == null) return;   // duplicate instance waiting to be destroyed

            // Attached voices follow their target; a destroyed target leaves the sound where it was.
            foreach (Voice v in voices)
            {
                if (!v.isAttached || !v.source.isPlaying) continue;
                if (v.follow == null) { v.isAttached = false; continue; }
                PlaceVoice(v, v.follow.position);
            }
        }

        // ── Sound effects ───────────────────────────────────────────────────────────

        // No position: UI clicks, global stingers.
        public static SoundHandle Play(SoundEvent sfx)                     => PlayStatic(sfx, null, null);
        // At a world position (also used for 2D screen panning).
        public static SoundHandle Play(SoundEvent sfx, Vector3 position)   => PlayStatic(sfx, position, null);
        // Follows a moving transform — engines, burning objects, looping auras.
        public static SoundHandle PlayAttached(SoundEvent sfx, Transform follow)
            => PlayStatic(sfx, follow != null ? follow.position : (Vector3?)null, follow);

        static SoundHandle PlayStatic(SoundEvent sfx, Vector3? position, Transform follow)
        {
            if (sfx == null || !sfx.HasClips) return SoundHandle.None;
            AudioManager manager = Instance;
            return manager != null ? manager.PlayInternal(sfx, position, follow) : SoundHandle.None;
        }

        SoundHandle PlayInternal(SoundEvent sfx, Vector3? position, Transform follow)
        {
            float now = Time.unscaledTime;
            if (!states.TryGetValue(sfx, out EventState state)) states[sfx] = state = new EventState();

            // Spam limits: too soon after the last play, or too many copies already playing.
            if (now - state.lastPlayTime < sfx.minInterval) return SoundHandle.None;
            if (sfx.maxSimultaneous > 0 && CountPlaying(sfx) >= sfx.maxSimultaneous) return SoundHandle.None;

            int clipIndex = sfx.PickClipIndex(state.lastClip);
            AudioClip clip = sfx.clips[clipIndex];
            if (clip == null) return SoundHandle.None;

            int index = FindVoice(sfx.priority);
            if (index < 0) return SoundHandle.None;   // every voice is busy with something more important

            state.lastClip     = clipIndex;
            state.lastPlayTime = now;

            Voice v = voices[index];
            CancelFade(v);
            v.source.Stop();
            v.generation++;
            v.sfx        = sfx;
            v.channel    = sfx.channel;
            v.follow     = follow;
            v.isAttached = follow != null;
            v.startTime  = now;

            v.source.clip = clip;
            sfx.ApplyTo(v.source, sfxGroup);
            v.baseVolume = v.source.volume;   // ApplyTo rolled this play's random volume
            v.fade       = 1f;
            ApplyVolume(v);

            if (position.HasValue) PlaceVoice(v, position.Value);
            else { v.source.transform.localPosition = Vector3.zero; v.source.panStereo = 0f; }
            v.source.Play();
            return new SoundHandle(this, index, v.generation);
        }

        // A free voice if there is one; otherwise the oldest voice playing something no more important.
        int FindVoice(int priority)
        {
            int oldest = -1;
            for (int i = 0; i < voices.Length; i++)
            {
                Voice v = voices[i];
                if (!v.source.isPlaying) return i;
                if (v.source.priority < priority) continue;   // lower number = more important
                if (oldest < 0 || v.startTime < voices[oldest].startTime) oldest = i;
            }
            return oldest;
        }

        int CountPlaying(SoundEvent sfx)
        {
            int count = 0;
            foreach (Voice v in voices)
                if (v.sfx == sfx && v.source.isPlaying) count++;
            return count;
        }

        void PlaceVoice(Voice v, Vector3 position)
        {
            v.source.transform.position = position;
            v.source.panStereo = v.source.spatialBlend < 1f ? ScreenPan(position) : 0f;
        }

        float ScreenPan(Vector3 worldPosition)
        {
            if (screenPan <= 0f) return 0f;
            Camera cam = Camera.main;
            if (cam == null) return 0f;
            float x = cam.WorldToViewportPoint(worldPosition).x;   // 0 = left edge, 1 = right edge
            return Mathf.Clamp((x - 0.5f) * 2f, -1f, 1f) * screenPan;
        }

        // Stops every sound effect. Music is unaffected.
        public static void StopAllSounds(float fadeOut = 0f)
        {
            if (instance == null) return;
            foreach (Voice v in instance.voices)
                if (v.source.isPlaying) instance.StopVoice(v, fadeOut);
        }

        // ── SoundHandle support ─────────────────────────────────────────────────────

        internal bool IsVoicePlaying(int index, int generation)
            => voices != null && index >= 0 && index < voices.Length
               && voices[index].generation == generation && voices[index].source.isPlaying;

        internal void StopVoice(int index, int generation, float fadeOut)
        {
            if (IsVoicePlaying(index, generation)) StopVoice(voices[index], fadeOut);
        }

        internal void SetVoicePosition(int index, int generation, Vector3 position)
        {
            if (!IsVoicePlaying(index, generation)) return;
            voices[index].isAttached = false;
            PlaceVoice(voices[index], position);
        }

        // ── Music ───────────────────────────────────────────────────────────────────

        // The music clip currently playing or fading in; null when stopped.
        public static AudioClip CurrentMusic { get; private set; }

        // Crossfades to a new track. Calling it again with the playing track only changes its volume.
        public static void PlayMusic(AudioClip clip, float fadeSeconds = 1f, float volume = 1f, bool loop = true)
        {
            AudioManager manager = Instance;
            if (manager == null || clip == null) return;

            Voice current = manager.music[manager.activeMusic];
            CurrentMusic = clip;
            if (current.source.clip == clip && current.source.isPlaying)
            {
                current.baseVolume = volume;
                manager.StartFade(current, 1f, fadeSeconds, stopAtEnd: false);   // also undoes a fade-out in progress
                return;
            }

            // Fade the old track out on its source and the new one in on the other.
            if (current.source.isPlaying) manager.StopVoice(current, fadeSeconds);
            manager.activeMusic = 1 - manager.activeMusic;
            Voice next = manager.music[manager.activeMusic];
            manager.CancelFade(next);
            next.source.clip = clip;
            next.source.loop = loop;
            next.baseVolume  = volume;
            next.fade        = fadeSeconds > 0f ? 0f : 1f;
            manager.ApplyVolume(next);
            next.source.Play();
            manager.StartFade(next, 1f, fadeSeconds, stopAtEnd: false);
        }

        public static void StopMusic(float fadeSeconds = 1f)
        {
            CurrentMusic = null;
            if (instance == null) return;
            Voice current = instance.music[instance.activeMusic];
            if (current.source.isPlaying) instance.StopVoice(current, fadeSeconds);
        }

        // ── Volume & pause ──────────────────────────────────────────────────────────

        // Final volume = the sound's own volume × its fade × the player's setting for its channel.
        // Master isn't multiplied here: VolumeSettings applies it through AudioListener.volume.
        void ApplyVolume(Voice v)
        {
            float setting = v.channel == VolumeChannel.Master ? 1f : VolumeSettings.Get(v.channel);
            v.source.volume = v.baseVolume * v.fade * setting;
        }

        void OnVolumeChanged(VolumeChannel channel, float volume)
        {
            if (channel == VolumeChannel.Master) return;
            foreach (Voice v in voices) if (v.channel == channel) ApplyVolume(v);
            foreach (Voice v in music)  if (v.channel == channel) ApplyVolume(v);
        }

        // For projects that use an AudioMixer instead of VolumeSettings: sets an exposed
        // mixer parameter from a 0–1 slider value (converted to decibels).
        public static void SetMixerVolume(AudioMixer mixer, string exposedParameter, float volume01)
        {
            if (mixer != null) mixer.SetFloat(exposedParameter, ToDecibels(volume01));
        }

        // 0–1 linear volume → decibels; -80 dB is silent.
        public static float ToDecibels(float volume01) => volume01 > 0.0001f ? Mathf.Log10(volume01) * 20f : -80f;

        // Pauses all audio except sounds marked "ignore listener pause" (and music, if set to ignore it).
        public static bool Paused
        {
            get => AudioListener.pause;
            set => AudioListener.pause = value;
        }

        // ── Fades ───────────────────────────────────────────────────────────────────

        void StopVoice(Voice v, float fadeOut)
        {
            if (fadeOut > 0f) StartFade(v, 0f, fadeOut, stopAtEnd: true);
            else { CancelFade(v); v.source.Stop(); }
        }

        void StartFade(Voice v, float target, float seconds, bool stopAtEnd)
        {
            CancelFade(v);
            if (seconds <= 0f)
            {
                v.fade = target;
                ApplyVolume(v);
                if (stopAtEnd) v.source.Stop();
                return;
            }
            v.fadeRoutine = StartCoroutine(Fade(v, target, seconds, stopAtEnd));
        }

        void CancelFade(Voice v)
        {
            if (v.fadeRoutine == null) return;
            StopCoroutine(v.fadeRoutine);
            v.fadeRoutine = null;
        }

        IEnumerator Fade(Voice v, float target, float seconds, bool stopAtEnd)
        {
            float start = v.fade;
            for (float t = 0f; t < seconds; t += Time.unscaledDeltaTime)
            {
                v.fade = Mathf.Lerp(start, target, t / seconds);
                ApplyVolume(v);
                yield return null;
            }
            v.fade = target;
            ApplyVolume(v);
            if (stopAtEnd) v.source.Stop();
            v.fadeRoutine = null;
        }
    }
}
