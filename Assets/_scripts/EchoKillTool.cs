using UnityEngine;

namespace _scripts
{
    // Held echo-killer: banishes the nearest echo within range each tick.
    public class EchoKillTool : HeldTool
    {
        [SerializeField] private EchoManager echoManager;

        public override void TickHeld(Vector2 holderPosition)
        {
            echoManager.KillNearest(holderPosition, range);
        }
    }
}
