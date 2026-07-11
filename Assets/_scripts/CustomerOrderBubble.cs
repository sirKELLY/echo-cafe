using UnityEngine;
using _scripts;

// On the Customer prefab. Shows one speech bubble above the head with the current order,
// and clears it the moment they're satisfied (or leave unserved). One bubble per customer.
//
// Spawned unparented (the seated customer never moves) and torn down explicitly on the two
// exit events, both of which fire before the customer's linger/destroy — so no orphaned bubbles.
[RequireComponent(typeof(Customer))]
public class CustomerOrderBubble : MonoBehaviour
{
    [SerializeField] private SpeechBubble bubblePrefab;
    [SerializeField] private Vector3 headOffset = new Vector3(0f, 1.75f, 0f);

    private Customer _customer;
    private SpeechBubble _bubble;

    private void Awake() => _customer = GetComponent<Customer>();

    private void OnEnable()
    {
        _customer.OnOrderChanged += HandleOrderChanged;
        _customer.OnItemAccepted += HandleItemAccepted;
        _customer.OnPaid += HandlePaid;
        _customer.OnLeftUnserved += HandleLeft;
    }

    private void OnDisable()
    {
        _customer.OnOrderChanged -= HandleOrderChanged;
        _customer.OnItemAccepted -= HandleItemAccepted;
        _customer.OnPaid -= HandlePaid;
        _customer.OnLeftUnserved -= HandleLeft;
    }

    private void HandleOrderChanged() => Refresh();
    private void HandleItemAccepted(ItemInfo item) => Refresh();   // an item was delivered -> show the next
    private void HandlePaid(int basePrice, int tip) => Clear();    // satisfied -> bubble goes away
    private void HandleLeft() => Clear();

    // Show the next item still owed; make the bubble on first need, tear it down when the order's empty.
    private void Refresh()
    {
        var items = _customer.RemainingItems;
        if (items == null || items.Count == 0) { Clear(); return; }

        if (_bubble == null && bubblePrefab != null)
            _bubble = Instantiate(bubblePrefab, transform.position + headOffset, Quaternion.identity);
        if (_bubble == null) return;

        _bubble.SetItem(items[0].sprite);
    }

    private void Clear()
    {
        if (_bubble != null) Destroy(_bubble.gameObject);
        _bubble = null;
    }
}
