using UnityEngine;

namespace _scripts
{
    // Recording start/stop cues. Goes next to RecordSource — every clip optional.
    [RequireComponent(typeof(RecordSource))]
    public class RecorderSfx : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        [SerializeField] private AudioClip recordingStarted;   // tape rolling
        [SerializeField] private AudioClip recordingStopped;   // tape out (echo spawn sound lives on GameSfx)

        private RecordSource _recorder;

        private void Awake() => _recorder = GetComponent<RecordSource>();

        private void OnEnable()
        {
            _recorder.OnRecordingStarted += PlayStarted;
            _recorder.OnRecordingStopped += PlayStopped;
        }

        private void OnDisable()
        {
            _recorder.OnRecordingStarted -= PlayStarted;
            _recorder.OnRecordingStopped -= PlayStopped;
        }

        private void PlayStarted() => Sfx.Play(recordingStarted, volume);
        private void PlayStopped() => Sfx.Play(recordingStopped, volume);
    }
}
