using System;
using UnityEngine;

namespace GameAudio
{
    // Volume groups a player can adjust separately. SoundEvents pick one; music uses Music.
    // Append new channels at the end — the value is saved in assets and PlayerPrefs by number.
    public enum VolumeChannel { Master, Music, Sfx, Ui, Ambience, Voice }

    // Player volume settings, one 0–1 value per channel, saved between sessions in PlayerPrefs.
    // Read and write it from a settings screen (or use the VolumeSlider component):
    //     VolumeSettings.Set(VolumeChannel.Music, 0.5f);
    //     float sfx = VolumeSettings.Get(VolumeChannel.Sfx);
    // Master drives AudioListener.volume; every other channel is applied by the AudioManager.
    public static class VolumeSettings
    {
        // Prefix for the PlayerPrefs keys, e.g. "GameAudio.Volume.Music".
        public static string KeyPrefix = "GameAudio.Volume.";
        // Value used for a channel that has never been set.
        public static float DefaultVolume = 1f;

        // Fired after a channel's volume changes.
        public static event Action<VolumeChannel, float> Changed;

        static float[] values;

        public static float Get(VolumeChannel channel)
        {
            EnsureLoaded();
            return values[(int)channel];
        }

        public static void Set(VolumeChannel channel, float volume01)
        {
            EnsureLoaded();
            volume01 = Mathf.Clamp01(volume01);
            values[(int)channel] = volume01;
            PlayerPrefs.SetFloat(Key(channel), volume01);
            if (channel == VolumeChannel.Master) AudioListener.volume = volume01;
            Changed?.Invoke(channel, volume01);
        }

        // Writes PlayerPrefs to disk now. Unity also does this on quit; call it when a settings screen closes.
        public static void Save() => PlayerPrefs.Save();

        // Puts every channel back to DefaultVolume.
        public static void ResetAll()
        {
            foreach (VolumeChannel channel in Enum.GetValues(typeof(VolumeChannel)))
                Set(channel, DefaultVolume);
        }

        static string Key(VolumeChannel channel) => KeyPrefix + channel;

        static void EnsureLoaded()
        {
            if (values != null) return;
            var channels = (VolumeChannel[])Enum.GetValues(typeof(VolumeChannel));
            values = new float[channels.Length];
            foreach (VolumeChannel channel in channels)
                values[(int)channel] = Mathf.Clamp01(PlayerPrefs.GetFloat(Key(channel), DefaultVolume));
        }

        // Clears statics when domain reload is disabled in Enter Play Mode settings.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() { values = null; Changed = null; }

        // Applies the saved master volume before the first scene loads.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ApplyMasterOnStartup() => AudioListener.volume = Get(VolumeChannel.Master);
    }
}
