using UnityEngine;

namespace GameAudio
{
    // A reference to one playing sound, returned by every Play call. Safe to hold onto:
    // once the sound ends or its voice is reused for another sound, the handle does nothing.
    public readonly struct SoundHandle
    {
        readonly AudioManager owner;
        readonly int voice;
        readonly int generation;

        internal SoundHandle(AudioManager owner, int voice, int generation)
        {
            this.owner      = owner;
            this.voice      = voice;
            this.generation = generation;
        }

        // Refers to nothing — returned when a sound was skipped (spam limit, no clips, all voices busy).
        public static SoundHandle None => default;

        public bool IsPlaying => owner != null && owner.IsVoicePlaying(voice, generation);

        // Stops the sound, optionally fading it out over fadeOut seconds. Use it for looping sounds.
        public void Stop(float fadeOut = 0f)
        {
            if (owner != null) owner.StopVoice(voice, generation, fadeOut);
        }

        // Moves a sound that isn't attached to a transform.
        public void SetPosition(Vector3 position)
        {
            if (owner != null) owner.SetVoicePosition(voice, generation, position);
        }
    }
}
