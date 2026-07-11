using UnityEngine;

namespace _scripts
{
    // Shows a "juice" object (particles / glow / whatever) while recording. Goes next to
    // RecordSource and mirrors RecorderSfx — on at record start, off at record end (whether
    // the record ends by key press or by the timer, since both fire OnRecordingStopped).
    [RequireComponent(typeof(RecordSource))]
    public class RecordingIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject juice;   // VFX container; hidden except while recording

        private RecordSource _recorder;

        private void Awake()
        {
            _recorder = GetComponent<RecordSource>();
            if (juice != null) juice.SetActive(false);   // start hidden
        }

        private void OnEnable()
        {
            _recorder.OnRecordingStarted += Show;
            _recorder.OnRecordingStopped += Hide;
        }

        private void OnDisable()
        {
            _recorder.OnRecordingStarted -= Show;
            _recorder.OnRecordingStopped -= Hide;
        }

        private void Show() { if (juice != null) juice.SetActive(true); }
        private void Hide() { if (juice != null) juice.SetActive(false); }
    }
}
