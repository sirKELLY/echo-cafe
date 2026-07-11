using System.Collections;
using UnityEngine;
using _scripts;

// Lives on the Customer prefab next to Customer. Turns the customer's discrete events
// (paid / wrong item / left) into floating popups above their head.
//
// Popups are spawned UNPARENTED on purpose so they outlive the customer. On pay they're
// dealt out one at a time (price -> tip -> emote), all from the same head position; the
// upward float of each earlier popup is what clears space for the next, so no offset is needed.
[RequireComponent(typeof(Customer))]
public class CustomerFeedback : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private FloatingText popupPrefab;
    [SerializeField] private Vector3 headOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Deal timing")]   // keep priceDelay + tipDelay under Customer.lingerSeconds or the emote won't spawn
    [SerializeField] private float priceDelay = 0.35f;   // beat after the price before the tip
    [SerializeField] private float tipDelay = 0.35f;     // beat after the tip before the emote

    [Header("Colors")]
    [SerializeField] private Color priceColor = new Color(0.4f, 1f, 0.4f);   // green
    [SerializeField] private Color tipColor = new Color(1f, 0.85f, 0.2f);    // gold

    [Header("Emote colors")]   // face tint by mood
    [SerializeField] private Color happyColor = new Color(0.4f, 1f, 0.5f);   // :D  tipped
    [SerializeField] private Color contentColor = Color.white;               // :)  served, no tip
    [SerializeField] private Color wrongColor = new Color(1f, 0.6f, 0.1f);   // :/  wrong item
    [SerializeField] private Color angryColor = new Color(1f, 0.3f, 0.3f);   // >:( patience ran out

    [Header("Sound")]   // all optional — an unassigned clip is silent
    [SerializeField] private AudioClip popupSfx;    // money popup — plays when a satisfied customer pays
    [SerializeField] private AudioClip thanksSfx;   // the satisfied emote  :D / :)
    [SerializeField] private AudioClip byeSfx;      // the unserved-leave emote  >:(
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private Customer _customer;

    private void Awake() => _customer = GetComponent<Customer>();

    private void OnEnable()
    {
        _customer.OnPaid += HandlePaid;
        _customer.OnWrongItem += HandleWrongItem;
        _customer.OnLeftUnserved += HandleLeft;
    }

    private void OnDisable()
    {
        _customer.OnPaid -= HandlePaid;
        _customer.OnWrongItem -= HandleWrongItem;
        _customer.OnLeftUnserved -= HandleLeft;
    }

    // Satisfied: deal price, then tip (if any), then the face — one at a time.
    private void HandlePaid(int basePrice, int tip)
    {
        Sfx.Play(popupSfx, sfxVolume);   // no-op if no clip is assigned
        StartCoroutine(DealPopups(basePrice, tip));
    }

    private IEnumerator DealPopups(int basePrice, int tip)
    {
        Spawn($"+${basePrice}", priceColor);
        yield return new WaitForSeconds(priceDelay);

        if (tip > 0)
        {
            Spawn($"+${tip} tip", tipColor);
            yield return new WaitForSeconds(tipDelay);
        }

        Spawn(tip > 0 ? ":D" : ":)", tip > 0 ? happyColor : contentColor);
        Sfx.Play(thanksSfx, sfxVolume);   // the emote's own voice, distinct from the money popup
    }

    private void HandleWrongItem(ItemInfo item) => Spawn(":/", wrongColor);   // comedy whiff
    private void HandleLeft()
    {
        Spawn(">:(", angryColor);         // patience ran out
        Sfx.Play(byeSfx, sfxVolume);
    }

    private void Spawn(string text, Color color)
    {
        if (popupPrefab == null) return;
        FloatingText popup = Instantiate(popupPrefab, transform.position + headOffset, Quaternion.identity);
        popup.SetText(text, color);
    }
}
