using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextPulseGlow : MonoBehaviour
{
    [Header("Pulse (scale)")]
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 1.08f;

    [Header("Glow (color/alpha)")]
    [SerializeField] private Color baseColor = new Color(1f, 0.85f, 0.4f); // warm gold
    [SerializeField] private float minAlpha = 0.7f;
    [SerializeField] private float maxAlpha = 1f;

    [Header("Timing")]
    [SerializeField] private float speed = 1.5f;

    private TextMeshProUGUI _text;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f; // 0 to 1

        // Scale pulse
        float scale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = new Vector3(scale, scale, scale);

        // Alpha "glow" pulse
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
        _text.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }
}