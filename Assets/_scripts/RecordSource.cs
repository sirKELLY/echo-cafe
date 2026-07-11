using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _scripts
{
    public struct EchoRecord
    {
        public List<SourceFrame> frames;
        public Vector2 anchor;
    }

    public class RecordSource: MonoBehaviour
    {
        [SerializeField] private EchoManager echoManager;
        [SerializeField] GameObject _player;
        [SerializeField] private float maxRecordSeconds = 5f;
        [SerializeField] private Key recordKey = Key.Space;

        private IIntentSource _playerSource;
        private EchoRecord _echoRecord;
        private bool _isRecording;
        private float _recordTimer;

        // recording is the game's most invisible state — surface it for the HUD
        public bool IsRecording => _isRecording;
        public float RecordProgress01 => _isRecording ? Mathf.Clamp01(_recordTimer / maxRecordSeconds) : 0f;

        public event System.Action OnRecordingStarted;
        public event System.Action OnRecordingStopped;   // key press or timer, either way

        private void Awake()
        {
            _playerSource = _player.GetComponent<IIntentSource>() as IIntentSource;
            Debug.Assert(_playerSource != null,
                "playerSourceObject must implement IIntentSource.");
            
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            // press to start, press again to stop (the timer also stops it, see FixedUpdate)
            if (Keyboard.current[recordKey].wasPressedThisFrame)
            {
                if (_isRecording) StopRecording();
                else StartRecording();
            }
        }

        private void FixedUpdate()
        {
            if (!_isRecording) return;

            _echoRecord.frames.Add(_playerSource.Sample());

            _recordTimer += Time.fixedDeltaTime;
            if (_recordTimer >= maxRecordSeconds) StopRecording();
        }

        public void StartRecording()
        {
            _isRecording = true;
            _recordTimer = 0f;
            _echoRecord = new EchoRecord
            {
                frames = new List<SourceFrame>(),
                anchor = _playerSource.CurrentPosition()
            };
            OnRecordingStarted?.Invoke();
        }

        public void StopRecording()
        {
            _isRecording = false;
            OnRecordingStopped?.Invoke();
            if (_echoRecord.frames == null || _echoRecord.frames.Count == 0) return;
            echoManager.SpawnEcho(_echoRecord.frames, _echoRecord.anchor);
        }
    }
}
