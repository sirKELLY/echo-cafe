using UnityEngine;
using UnityEngine.UI; 
public class MysticalPulse : MonoBehaviour
{
    [SerializeField] private RawImage shimmerImage; // uses a soft diagonal gradient texture
    [SerializeField] private float speed = 0.3f;

    void Update()
    {
        Rect uv = shimmerImage.uvRect;
        uv.x += speed * Time.deltaTime;
        if (uv.x > 1f) uv.x -= 1f;
        shimmerImage.uvRect = uv;
    }
}