using UnityEngine;
using UnityEngine.InputSystem;

namespace _scripts
{
    // Press the kill key to banish the nearest echo within range of the player.
    // Killing removes the echo from play, so it stops looping. Mirrors RecordSource:
    // raw key read, serialized player + manager refs (no Input-asset edits needed).
    public class EchoKiller : MonoBehaviour
    {
        [SerializeField] private EchoManager echoManager;
        [SerializeField] private GameObject player;
        [SerializeField] private float killRange = 1.5f;
        [SerializeField] private Key killKey = Key.K;

        private void Update()
        {
            if (Keyboard.current == null) return;
            if (!Keyboard.current[killKey].wasPressedThisFrame) return;

            echoManager.KillNearest(player.transform.position, killRange);
        }
    }
}
