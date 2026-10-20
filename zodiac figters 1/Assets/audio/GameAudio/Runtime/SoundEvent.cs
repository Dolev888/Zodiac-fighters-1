using UnityEngine;
using UnityEngine.Audio;

namespace GameAudio
{
    // A reusable sound definition: one or more clip variations plus playback settings.
    // Create via Assets → Create → Audio → Sound Event, then assign it to any SoundEvent field
    // and play it with sfx.Play() / sfx.Play(position) / sfx.PlayAttached(transform).
    [CreateAssetMenu(menuName = "Audio/Sound Event", fileName = "SFX_New", order = 0)]
    public class SoundEvent : ScriptableObject
    {
        [Tooltip("One clip is chosen at random each time the sound plays.")]
        public AudioClip[] clips;
        [Tooltip("Never pick the same clip twice in a row (needs 2+ clips).")]
        public bool avoidRepeat = true;

        [Header("Volume & Pitch")]
        [Range(0f, 1f)] public float volume = 1f;
        [Tooltip("Random amount subtracted from the volume on each play.")]
        [Range(0f, 1f)] public float volumeVariation = 0f;
        [Tooltip("Pitch is picked at random between min and max on each play. Small variation keeps repeated sounds from sounding robotic.")]
        [Range(0.1f, 3f)] public float pitchMin = 0.95f;
        [Range(0.1f, 3f)] public float pitchMax = 1.05f;
        public bool loop = false;

        [Header("Routing")]
        [Tooltip("Which volume setting this sound follows (see VolumeSettings / VolumeSlider).")]
        public VolumeChannel channel = VolumeChannel.Sfx;
        [Tooltip("Mixer group to play through. Empty = the AudioManager's default SFX group.")]
        public AudioMixerGroup mixerGroup;
        [Tooltip("0 = most important, 256 = least. When every voice is busy, less important sounds are cut first.")]
        [Range(0, 256)] public int priority = 128;
        [Tooltip("Keep playing while AudioListener.pause is true (e.g. pause-menu UI sounds).")]
        public bool ignoreListenerPause = false;

        [Header("Spatial")]
        [Tooltip("0 = 2D (same volume everywhere), 1 = fully 3D (fades and pans with distance from the listener).")]
        [Range(0f, 1f)] public float spatialBlend = 0f;
        [Tooltip("3D only: distance at which the sound starts getting quieter.")]
        [Min(0f)] public float minDistance = 1f;
        [Tooltip("3D only: distance at which the sound stops getting quieter.")]
        [Min(0f)] public float maxDistance = 30f;
        public AudioRolloffMode rolloff = AudioRolloffMode.Linear;

        [Header("Spam Limits")]
        [Tooltip("Maximum copies of this sound playing at the same time. 0 = unlimited.")]
        [Min(0)] public int maxSimultaneous = 4;
        [Tooltip("Minimum seconds between two plays of this sound (unscaled time). Extra plays are skipped.")]
        [Min(0f)] public float minInterval = 0.05f;

        public bool HasClips => clips != null && clips.Length > 0;

        public SoundHandle Play()                          => AudioManager.Play(this);
        public SoundHandle Play(Vector3 position)          => AudioManager.Play(this, position);
        public SoundHandle PlayAttached(Transform follow)  => AudioManager.PlayAttached(this, follow);

        // Random clip index, avoiding lastIndex when avoidRepeat is on.
        public int PickClipIndex(int lastIndex)
        {
            int count = clips.Length;
            if (count == 1) return 0;
            if (!avoidRepeat || lastIndex < 0 || lastIndex >= count) return Random.Range(0, count);
            int i = Random.Range(0, count - 1);
            return i >= lastIndex ? i + 1 : i;
        }

        // Copies this event's settings onto a source and rolls its random volume/pitch.
        // The clip is set separately by the caller. Channel volume is applied by the AudioManager.
        public void ApplyTo(AudioSource source, AudioMixerGroup fallbackGroup)
        {
            source.volume                = Mathf.Clamp01(volume - Random.Range(0f, volumeVariation));
            source.pitch                 = Random.Range(pitchMin, pitchMax);
            source.loop                  = loop;
            source.outputAudioMixerGroup = mixerGroup != null ? mixerGroup : fallbackGroup;
            source.priority              = priority;
            source.ignoreListenerPause   = ignoreListenerPause;
            source.spatialBlend          = spatialBlend;
            source.minDistance           = minDistance;
            source.maxDistance           = Mathf.Max(minDistance, maxDistance);
            source.rolloffMode           = rolloff;
        }

        void OnValidate()
        {
            if (pitchMax < pitchMin) pitchMax = pitchMin;
        }
    }
}
