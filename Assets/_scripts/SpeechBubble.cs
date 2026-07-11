using UnityEngine;

// On the speech-bubble prefab root (which draws the bubble background).
// Holds the child SpriteRenderer where the ordered item's icon is shown.
public class SpeechBubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer itemIcon;   // child renderer for the order item

    public void SetItem(Sprite sprite)
    {
        if (itemIcon == null) return;
        itemIcon.sprite = sprite;
        itemIcon.enabled = sprite != null;
    }
}
