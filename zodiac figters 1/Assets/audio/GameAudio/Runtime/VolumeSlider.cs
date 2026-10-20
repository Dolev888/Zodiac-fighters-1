using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameAudio
{
    // Connects a UI Slider to one volume channel. When the screen opens the slider shows the saved
    // value; moving it changes the volume immediately, and releasing it saves the setting.
    // Works with any slider range — the slider's min..max maps to 0..1 volume.
    [RequireComponent(typeof(Slider))]
    [AddComponentMenu("Audio/Volume Slider")]
    public class VolumeSlider : MonoBehaviour, IPointerUpHandler
    {
        public VolumeChannel channel = VolumeChannel.Master;
        [Tooltip("Optional sound played when the slider is released, so the player hears the new level. Use a sound on the same channel.")]
        public SoundEvent previewSound;

        Slider slider;

        void Awake()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnSliderChanged);
        }

        void OnEnable()
        {
            slider.SetValueWithoutNotify(Mathf.Lerp(slider.minValue, slider.maxValue, VolumeSettings.Get(channel)));
        }

        void OnDestroy()
        {
            if (slider != null) slider.onValueChanged.RemoveListener(OnSliderChanged);
        }

        void OnSliderChanged(float value) => VolumeSettings.Set(channel, slider.normalizedValue);

        public void OnPointerUp(PointerEventData eventData)
        {
            VolumeSettings.Save();
            if (previewSound != null) previewSound.Play();
        }
    }
}
