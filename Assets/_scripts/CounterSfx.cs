using UnityEngine;

namespace _scripts
{
    // Counter moments. Goes on each Counter — every clip optional.
    // Note: placing also fires CharacterSfx.itemPlacedOnCounter — assign the sound in ONE place.
    [RequireComponent(typeof(Counter))]
    public class CounterSfx : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        [SerializeField] private AudioClip itemPlaced;   // set down on this counter
        [SerializeField] private AudioClip itemTaken;    // picked up off this counter

        private Counter _counter;

        private void Awake() => _counter = GetComponent<Counter>();

        private void OnEnable()
        {
            _counter.OnItemPlaced += PlayPlaced;
            _counter.OnItemTaken += PlayTaken;
        }

        private void OnDisable()
        {
            _counter.OnItemPlaced -= PlayPlaced;
            _counter.OnItemTaken -= PlayTaken;
        }

        private void PlayPlaced(ItemInfo _) => Sfx.Play(itemPlaced, volume);
        private void PlayTaken(ItemInfo _) => Sfx.Play(itemTaken, volume);
    }
}
