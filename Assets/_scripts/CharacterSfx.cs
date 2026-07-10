using _scripts;
using UnityEngine;

// Hand/interact sounds. Goes on the player AND the echo prefab — every clip optional.
// Tip: set volume lower on the echo prefab so echoes sound like echoes.
[RequireComponent(typeof(ExecuteBehaviour))]
public class CharacterSfx : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    [SerializeField] private AudioClip itemReceived;          // craft done / picked up from counter
    [SerializeField] private AudioClip itemDelivered;         // customer took it
    [SerializeField] private AudioClip itemPlacedOnCounter;   // set down (also see CounterSfx — assign one, not both)
    [SerializeField] private AudioClip itemSpilled;           // splat!
    [SerializeField] private AudioClip interactWhiff;         // pressed at nothing, empty-handed

    private ExecuteBehaviour _execute;

    private void Awake() => _execute = GetComponent<ExecuteBehaviour>();

    private void OnEnable()
    {
        _execute.OnItemReceived += PlayReceived;
        _execute.OnItemDelivered += PlayDelivered;
        _execute.OnItemDropped += PlayPlaced;
        _execute.OnItemSpilled += PlaySpilled;
        _execute.OnInteractWhiff += PlayWhiff;
    }

    private void OnDisable()
    {
        _execute.OnItemReceived -= PlayReceived;
        _execute.OnItemDelivered -= PlayDelivered;
        _execute.OnItemDropped -= PlayPlaced;
        _execute.OnItemSpilled -= PlaySpilled;
        _execute.OnInteractWhiff -= PlayWhiff;
    }

    private void PlayReceived(ItemInfo _) => Sfx.Play(itemReceived, volume);
    private void PlayDelivered(ItemInfo _) => Sfx.Play(itemDelivered, volume);
    private void PlayPlaced(ItemInfo _) => Sfx.Play(itemPlacedOnCounter, volume);
    private void PlaySpilled(ItemInfo _) => Sfx.Play(itemSpilled, volume);
    private void PlayWhiff() => Sfx.Play(interactWhiff, volume);
}
