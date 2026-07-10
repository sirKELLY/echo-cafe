using System.Collections.Generic;
using UnityEngine;

namespace _scripts
{
    public class EchoManager: MonoBehaviour
    {
        [SerializeField] private GameObject echoPrefab;

        private readonly List<ReplaySource> _echoes = new();

        public event System.Action<ReplaySource> OnEchoSpawned;

        // live count for the HUD (prunes any echoes destroyed later)
        public int ActiveEchoCount
        {
            get { _echoes.RemoveAll(e => e == null); return _echoes.Count; }
        }

        // Spawns an echo and hands it the recorded data. The data travels; the component
        // that plays it already lives on the prefab.
        public void SpawnEcho(List<SourceFrame> frames, Vector2 anchor)
        {
            GameObject echo = Instantiate(echoPrefab);
            ReplaySource replay = echo.GetComponent<ReplaySource>();
            replay.SetupEcho(frames, anchor);
            _echoes.Add(replay);
            OnEchoSpawned?.Invoke(replay);
        }
    }
}
