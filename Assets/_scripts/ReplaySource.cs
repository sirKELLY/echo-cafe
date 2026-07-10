using System.Collections.Generic;
using UnityEngine;

namespace _scripts
{
    public class ReplaySource: MonoBehaviour, IIntentSource
    {
        private List<SourceFrame> frames = new List<SourceFrame>();
        private int frameCount = 0;
        [SerializeField] private Vector2 anchorA;

        private ExecuteBehaviour _execute;

        // for loop-clocks over echoes and shimmer effects at the anchor
        public float LoopProgress01 => frames.Count == 0 ? 0f : (float)frameCount / frames.Count;
        public float LoopDurationSeconds => frames.Count * Time.fixedDeltaTime;
        public Vector2 Anchor => anchorA;

        public event System.Action OnLoopRestarted;   // fires at the top of every loop (incl. the first)

        private void Awake()
        {
            _execute = GetComponent<ExecuteBehaviour>();
        }

        public void SetupEcho(List<SourceFrame> recordedFrames, Vector2 recordedAnchor)
        {
            frames = recordedFrames;
            anchorA = recordedAnchor;
            frameCount = 0;
            transform.position = (Vector3)anchorA;
        }

        // Called once per FixedUpdate by this echo's ExecuteBehaviour.
        public SourceFrame Sample()
        {
            if (frames.Count == 0) return default;

            if (frameCount == 0) BeginLoop();

            SourceFrame frame = frames[frameCount];

            ++frameCount;
            if (frameCount >= frames.Count) frameCount = 0; // wrap -> loop

            return frame;
        }

        // Top of every loop: reset to the start anchor and empty the hand, so an echo
        // never carries a leftover item into its next run.
        private void BeginLoop()
        {
            transform.position = (Vector3)anchorA;
            if (_execute != null && _execute.IsHoldingItem) _execute.Spill();
            OnLoopRestarted?.Invoke();
        }

        public Vector2 CurrentPosition()
        {
            return transform.position;
        }
    }
}
