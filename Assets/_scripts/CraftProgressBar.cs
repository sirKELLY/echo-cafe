using UnityEngine;

// Drop this prefab under any character (the player OR an echo). Shows a fill bar above
// the head while that character is crafting at a station. Auto-binds to the character's
// ExecuteBehaviour, so the player and every echo each drive their own bar independently.
//
// The script sits on an always-active root and toggles a child 'visual' — never SetActive
// the object this component lives on, or LateUpdate would stop and it could never re-show.
public class CraftProgressBar : MonoBehaviour
{
    [SerializeField] private ExecuteBehaviour source;   // auto-found from parent if left empty
    [SerializeField] private GameObject visual;         // bg + fill container; hidden when not crafting
    [SerializeField] private Transform fill;            // fill sprite, pivot on its LEFT edge; x-scaled 0..1

    void Awake()
    {
        if (source == null) source = GetComponentInParent<ExecuteBehaviour>();
    }

    void LateUpdate()
    {
        bool show = source != null && source.IsCrafting;
        if (visual.activeSelf != show) visual.SetActive(show);
        if (!show) return;

        Vector3 s = fill.localScale;
        s.x = Mathf.Clamp01(source.CraftProgress01);
        fill.localScale = s;
    }
}
