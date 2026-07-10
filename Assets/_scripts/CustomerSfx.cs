using _scripts;
using UnityEngine;

// Customer moments. Goes on the customer prefab — every clip optional.
[RequireComponent(typeof(Customer))]
public class CustomerSfx : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    [SerializeField] private AudioClip arrived;        // door chime (plays on spawn)
    [SerializeField] private AudioClip itemAccepted;   // right item handed over
    [SerializeField] private AudioClip wrongItem;      // "?!" — not on the order
    [SerializeField] private AudioClip paid;           // order complete, money in
    [SerializeField] private AudioClip tipped;         // extra sting when the tip was > 0
    [SerializeField] private AudioClip leftUnserved;   // patience out, storms off

    private Customer _customer;

    private void Awake() => _customer = GetComponent<Customer>();

    private void Start() => Sfx.Play(arrived, volume);

    private void OnEnable()
    {
        _customer.OnItemAccepted += PlayAccepted;
        _customer.OnWrongItem += PlayWrong;
        _customer.OnPaid += PlayPaid;
        _customer.OnLeftUnserved += PlayLeft;
    }

    private void OnDisable()
    {
        _customer.OnItemAccepted -= PlayAccepted;
        _customer.OnWrongItem -= PlayWrong;
        _customer.OnPaid -= PlayPaid;
        _customer.OnLeftUnserved -= PlayLeft;
    }

    private void PlayAccepted(ItemInfo _) => Sfx.Play(itemAccepted, volume);
    private void PlayWrong(ItemInfo _) => Sfx.Play(wrongItem, volume);
    private void PlayLeft() => Sfx.Play(leftUnserved, volume);

    private void PlayPaid(int basePrice, int tip)
    {
        Sfx.Play(paid, volume);
        if (tip > 0) Sfx.Play(tipped, volume);   // layers on top of the pay sound
    }
}
