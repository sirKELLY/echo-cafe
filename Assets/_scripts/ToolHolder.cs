using UnityEngine;
using UnityEngine.InputSystem;

namespace _scripts
{
    // Player-only. Grab the nearest tool from its stand, hold it (auto-running its effect on
    // an interval), and drop it back to its stand. Separate from the food hand slot, so a tool
    // never blocks crafting or serving.
    public class ToolHolder : MonoBehaviour
    {
        [SerializeField] private Transform handAnchor;        // where a held tool sits
        [SerializeField] private float pickupRange = 1.5f;    // reach to grab a tool off its stand
        [SerializeField] private float tickInterval = 0.25f;  // how often a held tool runs its effect
        [SerializeField] private Key toolKey = Key.E;         // grab if empty, drop (return to stand) if holding

        private HeldTool _held;
        private float _tickTimer;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current[toolKey].wasPressedThisFrame)
            {
                if (_held != null) Drop();
                else TryGrab();
            }

            if (_held == null) return;

            _tickTimer += Time.deltaTime;
            if (_tickTimer >= tickInterval)
            {
                _tickTimer = 0f;
                _held.TickHeld(transform.position);
            }
        }

        private void TryGrab()
        {
            HeldTool nearest = null;
            float best = pickupRange * pickupRange;
            foreach (var tool in FindObjectsByType<HeldTool>(FindObjectsSortMode.None))
            {
                if (tool.IsHeld) continue;
                float d = ((Vector2)tool.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (d <= best) { best = d; nearest = tool; }
            }
            if (nearest == null) return;

            nearest.PickUp(handAnchor);
            _held = nearest;
            _tickTimer = tickInterval;   // run the effect on the first frame held
        }

        private void Drop()
        {
            _held.ReturnHome();
            _held = null;
        }
    }
}
