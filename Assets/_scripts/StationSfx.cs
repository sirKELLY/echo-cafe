using UnityEngine;

namespace _scripts
{
    // Station moments. Goes on each WorkStation — clip optional.
    [RequireComponent(typeof(WorkStation))]
    public class StationSfx : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        [SerializeField] private AudioClip craftCompleted;   // ding! (also see CharacterSfx.itemReceived — assign one, not both)

        private WorkStation _station;

        private void Awake() => _station = GetComponent<WorkStation>();

        private void OnEnable() => _station.OnCraftCompleted += PlayCompleted;
        private void OnDisable() => _station.OnCraftCompleted -= PlayCompleted;

        private void PlayCompleted(ExecuteBehaviour _, ItemInfo __) => Sfx.Play(craftCompleted, volume);
    }
}
