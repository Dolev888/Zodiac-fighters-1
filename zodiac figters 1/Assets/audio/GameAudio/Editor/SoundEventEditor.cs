using UnityEditor;
using UnityEngine;

namespace GameAudio.EditorTools
{
    // Adds Preview / Stop buttons to the SoundEvent inspector so sounds can be tuned without entering Play mode.
    [CustomEditor(typeof(SoundEvent))]
    public class SoundEventEditor : UnityEditor.Editor
    {
        AudioSource previewSource;
        int lastClip = -1;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var sfx = (SoundEvent)target;
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!sfx.HasClips))
                    if (GUILayout.Button("Preview")) Preview(sfx);
                if (GUILayout.Button("Stop", GUILayout.Width(60))) StopPreview();
            }
            if (!sfx.HasClips)
                EditorGUILayout.HelpBox("Add at least one clip.", MessageType.Info);
        }

        void Preview(SoundEvent sfx)
        {
            if (previewSource == null)
                previewSource = EditorUtility.CreateGameObjectWithHideFlags(
                    "SoundEvent Preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();

            lastClip = sfx.PickClipIndex(lastClip);
            AudioClip clip = sfx.clips[lastClip];
            if (clip == null) return;

            previewSource.clip = clip;
            sfx.ApplyTo(previewSource, null);
            previewSource.spatialBlend = 0f;   // previews always play 2D
            previewSource.loop         = false;
            previewSource.Play();
        }

        void StopPreview()
        {
            if (previewSource != null) previewSource.Stop();
        }

        void OnDisable()
        {
            if (previewSource != null) DestroyImmediate(previewSource.gameObject);
        }
    }
}
