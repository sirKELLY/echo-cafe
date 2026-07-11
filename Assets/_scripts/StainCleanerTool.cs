using UnityEngine;

namespace _scripts
{
    // Held cleaner: removes every stain within range each tick.
    public class StainCleanerTool : HeldTool
    {
        public override void TickHeld(Vector2 holderPosition)
        {
            float r2 = range * range;
            foreach (var stain in FindObjectsByType<Stain>(FindObjectsSortMode.None))
            {
                if (((Vector2)stain.transform.position - holderPosition).sqrMagnitude <= r2)
                    Destroy(stain.gameObject);
            }
        }
    }
}
