using UnityEngine;
using GameAudio;

// Plays this scene's song when the scene starts.
public class SceneMusic : MonoBehaviour
{
    public AudioClip song;
    public float fadeSeconds = 1.5f;
    [Range(0f, 1f)] public float volume = 0.8f;

    void Start()
    {
        AudioManager.PlayMusic(song, fadeSeconds, volume);
   
    
    }
}


