using System.Collections.Generic;
using UnityEngine;

namespace _scripts
{
    public class ReplaySource: MonoBehaviour, IIntentSource
    {
        private List<SourceFrame> frames = new List<SourceFrame>();
        private int frameCount = 0;
        [SerializeField] private Vector2 anchorA;

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

            // snap back to the start anchor at the top of each loop
            if (frameCount == 0) transform.position = (Vector3)anchorA;

            SourceFrame frame = frames[frameCount];

            ++frameCount;
            if (frameCount >= frames.Count) frameCount = 0; // wrap -> loop

            return frame;
        }

        public Vector2 CurrentPosition()
        {
            return transform.position;
        }
    }
}
