using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GlowRing : MonoBehaviour
{
    [Header("Glow Color")]
    [SerializeField] private Color ringColor = new Color(0.4f, 0.9f, 1f); // cyan-ish

    [Header("Pulse")]
    [SerializeField] private float minAlpha = 0.5f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float minScale = 0.95f;
    [SerializeField] private float maxScale = 1.08f;
    [SerializeField] private float speed = 2f;

    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        gameObject.SetActive(true); // hidden until selected
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;

        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
        _sr.color = new Color(ringColor.r, ringColor.g, ringColor.b, alpha);

        float scale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
