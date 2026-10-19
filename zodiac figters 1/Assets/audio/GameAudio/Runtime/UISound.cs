using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameAudio
{
    // Plays sounds for a UI element: on click / submit and on hover.
    // Add it next to a Button (or any Selectable); it stays silent while the element isn't interactable.
    [AddComponentMenu("Audio/UI Sound")]
    public class UISound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISubmitHandler
    {
        [Tooltip("Played when the element is clicked or submitted with keyboard/gamepad.")]
        public SoundEvent clickSound;
        [Tooltip("Played when the pointer enters the element.")]
        public SoundEvent hoverSound;

        Selectable selectable;

        void Awake() => selectable = GetComponent<Selectable>();

        bool Interactable => selectable == null || selectable.IsInteractable();

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Interactable && hoverSound != null) hoverSound.Play();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && Interactable && clickSound != null)
                clickSound.Play();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (Interactable && clickSound != null) clickSound.Play();
        }
    }
}
