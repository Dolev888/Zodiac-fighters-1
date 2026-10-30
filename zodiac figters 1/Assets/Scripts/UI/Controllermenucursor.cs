using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Lets a controller use every menu like a mouse.
///   left stick  = moves a cursor on the screen
///   X / A       = clicks what is under the cursor
/// It works on ANY UI button that the mouse can use (main menu, command lists, character selection,
/// pause menu, win/lose panel, the upgrade screen...), so nothing has to be set up per panel.
///
/// Nothing needs to be added to a scene: it creates itself when the game starts and survives scene changes.
///
/// When is the cursor active? Only in "menu mode":
///   - in any scene that is not the match scene (main menu, character selection), or
///   - while the game is paused (Time.timeScale == 0: pause menu, win/lose panel, upgrade screen).
/// During a fight the stick moves the character as usual and the cursor is hidden.
/// If the real mouse moves, the controller cursor hides and the mouse is used as always.
/// </summary>
public class ControllerMenuCursor : MonoBehaviour
{
    private static ControllerMenuCursor instance;

    // the scene where the fight happens (the cursor is hidden there unless the game is paused)
    private const string MatchScene = "GamePlayScene";

    private const float StickDeadzone = 0.25f;
    private const float ScreenHeightsPerSecond = 0.9f;   // cursor speed at full stick: 0.9 screen heights per second

    // arrow picture size in pixels (drawn by code, no image file needed)
    private const int ArrowW = 24;
    private const int ArrowH = 32;
    private const float ArrowTipX = 2f;
    private const float ArrowTipY = 30f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null) return;

        var go = new GameObject(nameof(ControllerMenuCursor));
        DontDestroyOnLoad(go);
        go.AddComponent<ControllerMenuCursor>();
    }

    private RectTransform cursorRect;
    private Image cursorImage;

    private Vector2 cursorPos;
    private bool cursorPlaced;
    private bool usingPad;
    private bool wasActive;

    private GameObject hovered;
    private PointerEventData pointer;
    private readonly List<RaycastResult> results = new List<RaycastResult>();

    // ================================================================ unity
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        BuildCursor();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hovered = null;   // the old scene's buttons are gone
    }

    private void Update()
    {
        Gamepad pad = Gamepad.current;
        Mouse mouse = Mouse.current;

        bool menuMode = Time.timeScale == 0f || SceneManager.GetActiveScene().name != MatchScene;

        Vector2 stick = pad != null ? pad.leftStick.ReadValue() : Vector2.zero;
        bool stickMoved = stick.magnitude > StickDeadzone;
        bool clickPressed = pad != null && pad.buttonSouth.wasPressedThisFrame;

        // who is steering: the real mouse takes the cursor back as soon as it moves or clicks
        if (mouse != null && (mouse.delta.ReadValue().sqrMagnitude > 0.25f || mouse.leftButton.wasPressedThisFrame))
            usingPad = false;

        // the controller takes over when the stick moves or X/A is pressed in a menu
        if (menuMode && pad != null && (stickMoved || clickPressed) && !usingPad)
        {
            usingPad = true;
            // start where the mouse is, or in the middle of the screen
            cursorPos = mouse != null ? mouse.position.ReadValue() : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            cursorPlaced = true;
        }

        bool active = menuMode && usingPad && pad != null;

        if (active != wasActive)
        {
            wasActive = active;
            cursorImage.enabled = active;
            Cursor.visible = !active;   // hide the arrow of the mouse while the controller steers

            if (!active) SetHover(null);
        }

        if (!active) return;

        if (!cursorPlaced)
        {
            cursorPos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            cursorPlaced = true;
        }

        // move (the curve makes small stick movements finer)
        if (stickMoved)
        {
            Vector2 s = stick * stick.magnitude;
            cursorPos += s * (Screen.height * ScreenHeightsPerSecond) * Time.unscaledDeltaTime;
            cursorPos.x = Mathf.Clamp(cursorPos.x, 0f, Screen.width);
            cursorPos.y = Mathf.Clamp(cursorPos.y, 0f, Screen.height);
        }

        // draw it (same size feel at every resolution)
        float scale = Mathf.Max(1f, Screen.height / 1080f * 1.6f);
        cursorRect.sizeDelta = new Vector2(ArrowW, ArrowH) * scale;
        cursorRect.position = cursorPos;

        UpdateHoverAndClick(clickPressed);
    }

    // ================================================================ hover and click
    private void UpdateHoverAndClick(bool clickPressed)
    {
        if (EventSystem.current == null) return;

        pointer = new PointerEventData(EventSystem.current) { position = cursorPos, button = PointerEventData.InputButton.Left };

        results.Clear();
        EventSystem.current.RaycastAll(pointer, results);

        GameObject top = results.Count > 0 ? results[0].gameObject : null;
        if (results.Count > 0) pointer.pointerCurrentRaycast = results[0];

        // what is under the cursor and reacts to it (a button, even if the cursor is on its text)
        GameObject hoverTarget = top != null ? ExecuteEvents.GetEventHandler<IPointerEnterHandler>(top) : null;
        SetHover(hoverTarget);

        if (!clickPressed || top == null) return;

        GameObject clickTarget = ExecuteEvents.GetEventHandler<IPointerClickHandler>(top);
        if (clickTarget == null) return;

        // the same sequence a mouse click sends
        pointer.pointerPress = clickTarget;
        pointer.rawPointerPress = clickTarget;
        pointer.pointerPressRaycast = pointer.pointerCurrentRaycast;
        pointer.clickCount = 1;

        ExecuteEvents.Execute(clickTarget, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(clickTarget, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(clickTarget, pointer, ExecuteEvents.pointerClickHandler);
    }

    // makes the button under the cursor light up like it does under the mouse
    private void SetHover(GameObject target)
    {
        if (target == hovered) return;

        var data = pointer != null ? pointer : new PointerEventData(EventSystem.current) { position = cursorPos };

        if (hovered != null) ExecuteEvents.Execute(hovered, data, ExecuteEvents.pointerExitHandler);
        hovered = target;
        if (hovered != null) ExecuteEvents.Execute(hovered, data, ExecuteEvents.pointerEnterHandler);
    }

    // ================================================================ the cursor picture
    private void BuildCursor()
    {
        var canvasObject = new GameObject("ControllerCursorCanvas");
        canvasObject.transform.SetParent(transform, false);

        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;   // above every other canvas

        var imageObject = new GameObject("Cursor");
        imageObject.transform.SetParent(canvasObject.transform, false);

        cursorImage = imageObject.AddComponent<Image>();
        cursorImage.sprite = MakeArrowSprite();
        cursorImage.raycastTarget = false;      // the cursor must never block the buttons under it
        cursorImage.enabled = false;

        cursorRect = cursorImage.rectTransform;
        cursorRect.anchorMin = Vector2.zero;
        cursorRect.anchorMax = Vector2.zero;
        cursorRect.pivot = new Vector2(ArrowTipX / ArrowW, ArrowTipY / ArrowH);   // the tip of the arrow is the click point
        cursorRect.sizeDelta = new Vector2(ArrowW, ArrowH);
    }

    // a white arrow with a black outline, drawn pixel by pixel
    private static Sprite MakeArrowSprite()
    {
        Vector2[] poly =
        {
            new Vector2(2, 30), new Vector2(2, 7), new Vector2(7, 11), new Vector2(11, 2),
            new Vector2(15, 4), new Vector2(11, 12), new Vector2(17, 13)
        };

        bool[,] inside = new bool[ArrowW, ArrowH];
        for (int x = 0; x < ArrowW; x++)
            for (int y = 0; y < ArrowH; y++)
                inside[x, y] = PointInPolygon(new Vector2(x + 0.5f, y + 0.5f), poly);

        var pixels = new Color32[ArrowW * ArrowH];
        for (int x = 0; x < ArrowW; x++)
        {
            for (int y = 0; y < ArrowH; y++)
            {
                Color32 c = new Color32(0, 0, 0, 0);

                if (inside[x, y])
                {
                    c = new Color32(255, 255, 255, 255);
                }
                else
                {
                    // black outline: next to a white pixel
                    for (int dx = -1; dx <= 1 && c.a == 0; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = x + dx, ny = y + dy;
                            if (nx >= 0 && nx < ArrowW && ny >= 0 && ny < ArrowH && inside[nx, ny])
                            {
                                c = new Color32(0, 0, 0, 255);
                                break;
                            }
                        }
                    }
                }

                pixels[y * ArrowW + x] = c;
            }
        }

        var texture = new Texture2D(ArrowW, ArrowH, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels32(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, ArrowW, ArrowH), new Vector2(ArrowTipX / ArrowW, ArrowTipY / ArrowH), 100f);
    }

    private static bool PointInPolygon(Vector2 p, Vector2[] poly)
    {
        bool result = false;
        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
        {
            if ((poly[i].y > p.y) != (poly[j].y > p.y) &&
                p.x < (poly[j].x - poly[i].x) * (p.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x)
            {
                result = !result;
            }
        }
        return result;
    }
}