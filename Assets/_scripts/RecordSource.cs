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
        [SerializeField] private MonoBehaviour playerSourceObject; // must implement IIntentSource
        [SerializeField] private float maxRecordSeconds = 5f;
        [SerializeField] private Key recordKey = Key.R;

        private IIntentSource _playerSource;
        private EchoRecord _echoRecord;
        private bool _isRecording;
        private float _recordTimer;

        private void Awake()
        {
            _playerSource = playerSourceObject as IIntentSource;
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
        }

        public void StopRecording()
        {
            _isRecording = false;
            if (_echoRecord.frames == null || _echoRecord.frames.Count == 0) return;
            echoManager.SpawnEcho(_echoRecord.frames, _echoRecord.anchor);
        }
    }
}
