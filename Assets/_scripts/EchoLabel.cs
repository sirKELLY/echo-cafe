using TMPro;
using UnityEngine;

namespace _scripts
{
    // Connector: writes the echo's assigned number onto the TextMeshPro on its head.
    // Mirrors EchoSfx — goes on the echo prefab and subscribes to a ReplaySource event.
    [RequireComponent(typeof(ReplaySource))]
    public class EchoLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;   // the text mesh on the echo's head

        private ReplaySource _replay;

        private void Awake() => _replay = GetComponent<ReplaySource>();

        private void OnEnable()
        {
            _replay.OnEchoNumberAssigned += SetNumber;
            SetNumber(_replay.EchoNumber);   // defensive: in case the number landed before we subscribed
        }

        private void OnDisable() => _replay.OnEchoNumberAssigned -= SetNumber;

        private void SetNumber(int number)
        {
            if (label != null) label.text = number.ToString();
        }
    }
}
