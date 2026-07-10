using System.Collections.Generic;
using UnityEngine;

namespace _scripts
{
    public class EchoManager: MonoBehaviour
    {
        [SerializeField] private GameObject echoPrefab;

        private readonly List<ReplaySource> _echoes = new();
        private int _echoNumber;   // running total; each spawned echo keeps its number for life

        public event System.Action<ReplaySource> OnEchoSpawned;
        public event System.Action<ReplaySource> OnEchoKilled;   // fires before the echo is destroyed (poof / sfx hook)

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
            replay.SetupEcho(frames, anchor, ++_echoNumber);
            _echoes.Add(replay);
            OnEchoSpawned?.Invoke(replay);
        }

        // Destroy the nearest live echo within maxRange of point. Killing removes it from
        // play, so it stops looping. Returns true if an echo was in range and died.
        public bool KillNearest(Vector2 point, float maxRange)
        {
            _echoes.RemoveAll(e => e == null);

            ReplaySource nearest = null;
            float best = maxRange * maxRange;
            foreach (var echo in _echoes)
            {
                float d = ((Vector2)echo.transform.position - point).sqrMagnitude;
                if (d <= best)
                {
                    best = d;
                    nearest = echo;
                }
            }

            if (nearest == null) return false;
            KillEcho(nearest);
            return true;
        }

        // Remove one echo from play (fires the hook before it's gone so a poof can read its position).
        public void KillEcho(ReplaySource echo)
        {
            if (echo == null) return;
            _echoes.Remove(echo);
            OnEchoKilled?.Invoke(echo);
            Destroy(echo.gameObject);
        }
    }
}
