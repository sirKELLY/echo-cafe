using System.Collections;
using UnityEngine;
using _scripts;

// Lives on the Customer prefab next to Customer. Turns the customer's discrete events
// (paid / wrong item / left) into floating popups above their head.
//
// Popups are spawned UNPARENTED on purpose: the customer Destroys itself the instant it
// pays, so anything parented to it (or any coroutine on it) would die mid-animation.
// Everything is spawned at once and separated by height, letting the float-up read as a stack.
[RequireComponent(typeof(Customer))]
public class CustomerFeedback : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private FloatingText popupPrefab;
    [SerializeField] private Vector3 headOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private float stackGap = 0.4f;   // vertical spacing so stacked popups don't overlap

    [Header("Deal timing")]   // keep priceDelay + tipDelay under Customer.lingerSeconds or the emote won't spawn
    [SerializeField] private float priceDelay = 0.35f;   // beat after the price before the tip
    [SerializeField] private float tipDelay = 0.35f;     // beat after the tip before the emote

    [Header("Colors")]
    [SerializeField] private Color priceColor = new Color(0.4f, 1f, 0.4f);   // green
    [SerializeField] private Color tipColor = new Color(1f, 0.85f, 0.2f);    // gold
    [SerializeField] private Color emoteColor = Color.white;

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
    private void HandlePaid(int basePrice, int tip) => StartCoroutine(DealPopups(basePrice, tip));

    private IEnumerator DealPopups(int basePrice, int tip)
    {
        Spawn($"+${basePrice}", priceColor, 0);
        yield return new WaitForSeconds(priceDelay);

        if (tip > 0)
        {
            Spawn($"+${tip} tip", tipColor, 1);
            yield return new WaitForSeconds(tipDelay);
        }

        Spawn(tip > 0 ? ":D" : ":)", emoteColor, tip > 0 ? 2 : 1);
    }

    private void HandleWrongItem(ItemInfo item) => Spawn(":/", emoteColor, 0);   // comedy whiff
    private void HandleLeft() => Spawn(">:(", emoteColor, 0);                     // patience ran out

    private void Spawn(string text, Color color, int row)
    {
        if (popupPrefab == null) return;
        Vector3 pos = transform.position + headOffset + Vector3.up * (stackGap * row);
        FloatingText popup = Instantiate(popupPrefab, pos, Quaternion.identity);
        popup.SetText(text, color);
    }
}
