using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Hard pause: freezes time so movement, physics, the game clock and every time-based
// coroutine stop together. Wire a pause-menu panel's SetActive to onPaused / onResumed in
// the inspector, or call the public methods from a UI Button.
public class PauseController : MonoBehaviour
{
    [SerializeField] private bool pauseWithEscape = true;

    [SerializeField] private UnityEvent onPaused;    // e.g. show the pause panel
    [SerializeField] private UnityEvent onResumed;   // e.g. hide the pause panel

    public bool IsPaused { get; private set; }

    // Update runs even at timeScale 0, and key polling isn't time-scaled, so this still
    // fires while paused — letting the same key toggle back out.
    private void Update()
    {
        if (pauseWithEscape && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;
        Time.timeScale = 0f;
        onPaused?.Invoke();
    }

    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;
        Time.timeScale = 1f;
        onResumed?.Invoke();
    }
}
