using UnityEngine;

namespace _scripts
{
    // Echo loop cue. Goes on the echo prefab — clip optional.
    // Note: OnLoopRestarted also fires on the echo's very first frame (its "materialize" moment).
    [RequireComponent(typeof(ReplaySource))]
    public class EchoSfx : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        [SerializeField] private AudioClip loopRestarted;   // whoosh back to the anchor

        private ReplaySource _replay;

        private void Awake() => _replay = GetComponent<ReplaySource>();

        private void OnEnable() => _replay.OnLoopRestarted += PlayLoop;
        private void OnDisable() => _replay.OnLoopRestarted -= PlayLoop;

        private void PlayLoop() => Sfx.Play(loopRestarted, volume);
    }
}
