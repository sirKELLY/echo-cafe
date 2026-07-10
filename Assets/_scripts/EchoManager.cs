using System.Collections.Generic;
using UnityEngine;

namespace _scripts
{
    public class EchoManager: MonoBehaviour
    {
        [SerializeField] private GameObject echoPrefab;

        // Spawns an echo and hands it the recorded data. The data travels; the component
        // that plays it already lives on the prefab.
        public void SpawnEcho(List<SourceFrame> frames, Vector2 anchor)
        {
            GameObject echo = Instantiate(echoPrefab);
            echo.GetComponent<ReplaySource>().SetupEcho(frames, anchor);
        }
    }
}
