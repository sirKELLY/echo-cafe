using UnityEngine;

// One per scene. Draws a ring on whatever the PLAYER would interact with.
// Pull-based: only the referenced player is scanned, so echoes never get a ring.
public class InteractionSelector : MonoBehaviour
{
    [SerializeField] private ExecuteBehaviour player;   // auto-binds to the GetPlayerInput owner if left empty
    [SerializeField] private SpriteRenderer ring;       // the selector sprite
    [SerializeField] private float yOffset = 0f;        // nudge onto the target's base if its pivot is centered
    [SerializeField] private bool pulse = true;         // subtle idle pulse

    void Awake()
    {
        if (player == null)
        {
            var input = FindFirstObjectByType<GetPlayerInput>();   // player-only: echoes have ReplaySource
            if (input != null) player = input.GetComponent<ExecuteBehaviour>();
        }
    }

    void LateUpdate()
    {
        Transform target = player != null ? player.PeekNearestView() : null;
        ring.enabled = target != null;
        if (target == null) return;

        ring.transform.position = target.position + Vector3.up * yOffset;
        if (pulse) ring.transform.localScale = Vector3.one * (1f + 0.08f * Mathf.Sin(Time.time * 6f));
    }
}
