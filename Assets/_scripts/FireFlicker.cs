using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class FireFlicker : MonoBehaviour
{
    [SerializeField] private float intensityA = 1.5f;
    [SerializeField] private float intensityB = 2.2f;
    [SerializeField] private float flickerInterval = 0.15f; // time between switches

    private Light2D _light;
    private float _timer;
    private bool _onA = true;

    void Awake()
    {
        _light = GetComponent<Light2D>();
        _light.intensity = intensityA;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= flickerInterval)
        {
            _timer = 0f;
            _onA = !_onA;
            _light.intensity = _onA ? intensityA : intensityB;
        }
    }
}