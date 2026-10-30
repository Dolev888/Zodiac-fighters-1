using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;   // controller

// Put this on a PANEL (a command list, a win/lose panel...).
// While the panel is open, pressing the East button (Circle on PlayStation, B on Xbox) runs the "On Back" event.
// In the Inspector: On Back -> drag the object that has the script -> pick the method that closes this panel.
//   Main menu keyboard command list   -> MainMenu.CloseCommandList
//   Main menu controller command list -> MainMenu.CloseControllerCommandList
//   Pause menu keyboard list          -> PauseMenu.CloseCommandList
//   Pause menu controller list        -> PauseMenu.CloseControllerCommandList
// The panel is only active while it is open, so this only listens then, and it keeps working when the
// script that opened the panel was switched off together with the menu behind it.
public class ControllerBackButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onBack;
    [SerializeField] private bool debugLog = true;   // prints to the Console; untick or delete when it works

    private int framePanelOpened;

    private void OnEnable()
    {
        // ignore the press that opened the panel, if there was one
        framePanelOpened = Time.frameCount;
    }

    private void Update()
    {
        if (Time.frameCount == framePanelOpened) return;

        Gamepad pad = Gamepad.current;
        if (pad != null && pad.buttonEast.wasPressedThisFrame)
        {
            if (debugLog) Debug.Log("ControllerBackButton on '" + name + "': Circle pressed, calling " + onBack.GetPersistentEventCount() + " method(s)");
            onBack.Invoke();
        }
    }
}