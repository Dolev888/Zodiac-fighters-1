using UnityEngine;

namespace GameAudio.Examples
{
    // A small example of the GameAudio module. Put it on any object, drag Sound Events and a music
    // clip into its fields, press Play, and try the keys:
    //   Space – plays a one-shot sound at this object's position
    //   E     – hold to play a looping sound that follows this object; release to fade it out
    //   M     – starts the music (crossfades if another track is playing)
    [AddComponentMenu("Audio/Examples/Play Sound Example")]
    public class PlaySoundExample : MonoBehaviour
    {
        [Tooltip("A short sound, e.g. a jump or a hit.")]
        public SoundEvent oneShot;
        [Tooltip("A Sound Event with Loop ticked, e.g. an engine or a fire.")]
        public SoundEvent loop;
        [Tooltip("A music track.")]
        public AudioClip music;

        SoundHandle loopHandle;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && oneShot != null)
                oneShot.Play(transform.position);

            if (Input.GetKeyDown(KeyCode.E) && loop != null)
                loopHandle = loop.PlayAttached(transform);
            if (Input.GetKeyUp(KeyCode.E))
                loopHandle.Stop(0.3f);   // fade out over 0.3 seconds

            if (Input.GetKeyDown(KeyCode.M) && music != null)
                AudioManager.PlayMusic(music, fadeSeconds: 2f);
        }
    }
}
