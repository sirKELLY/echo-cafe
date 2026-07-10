using TMPro;
using UnityEngine;

// Drop this on a prefab that has a TMP_Text (world-space TextMeshPro, or a UI TextMeshProUGUI).
// Instantiate the prefab where you want a "+$n" to pop: it drifts upward, fades out, then deletes itself.
[RequireComponent(typeof(TMP_Text))]
public class FloatingText : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;      // seconds until it's gone
    [SerializeField] private float floatHeight = 1f;   // world units it drifts upward over its life

    private TMP_Text _text;
    private Vector3 _start;
    private float _elapsed;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _start = transform.position;
    }

    // Optional: set the label at spawn, e.g. Instantiate(prefab, pos, ...).GetComponent<FloatingText>().SetText($"+${amount}");
    public void SetText(string value)
    {
        if (_text == null) _text = GetComponent<TMP_Text>();   // in case this runs before Awake
        _text.text = value;
    }

    // Same, but tints the popup (the fade in Update still drives its alpha).
    public void SetText(string value, Color color)
    {
        SetText(value);
        _text.color = color;
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        float t = _elapsed / lifetime;

        transform.position = _start + Vector3.up * (floatHeight * t);
        _text.alpha = 1f - t;                              // full opacity -> transparent

        if (t >= 1f) Destroy(gameObject);
    }
}
