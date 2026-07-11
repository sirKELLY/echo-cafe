using System.Collections.Generic;
using UnityEngine;
using _scripts;

public class Customer : MonoBehaviour, IInteractable
{
    [Header("Patience / happiness (balance levers)")]
    [SerializeField] private float patienceSeconds = 20f;                 // time until happiness hits 0
    [SerializeField, Range(0f, 1f)] private float tipThreshold = 0.5f;    // happiness above this -> tip
    [SerializeField] private int maxTip = 5;
    [SerializeField] private float lingerSeconds = 1.2f;                  // stay seated + idling this long after paying, then vanish

    private Order _order;
    private float _waitTimer;
    private bool _done;   // paid or left: ignore further deliveries

    // 1 at arrival, 0 once patience is spent.
    public float Happiness => Mathf.Clamp01(1f - _waitTimer / patienceSeconds);

    public float PatienceRemainingSeconds => Mathf.Max(0f, patienceSeconds - _waitTimer);
    public IReadOnlyList<ItemInfo> RemainingItems => _order?.Remaining;   // the order bubble reads this

    // discrete moments for popups / stingers; all fire before any Destroy
    public event System.Action<ItemInfo> OnItemAccepted;   // right item handed over
    public event System.Action<ItemInfo> OnWrongItem;      // whiff: not on the order (comedy hook)
    public event System.Action<int, int> OnPaid;           // (basePrice, tip) — split so the tip can teach
    public event System.Action OnLeftUnserved;             // patience out, sale lost
    public event System.Action OnOrderChanged;             // order set — the head bubble builds itself on this

    public void SetOrder(IEnumerable<ItemInfo> items)
    {
        _order = new Order(items);
        OnOrderChanged?.Invoke();
    }

    private void Update()
    {
        if (_done) return;

        _waitTimer += Time.deltaTime;
        if (_waitTimer >= patienceSeconds)             // patience out -> lost sale
        {
            OnLeftUnserved?.Invoke();
            Leave(0);
        }
    }

    // IInteractable: same "walk up and hold interact" verb as a WorkStation.
    // Hand-over is instant, so this returns 1 on a successful delivery, 0 otherwise.
    public float Interact(ExecuteBehaviour user)
    {
        if (_done || _order == null) return 0f;
        if (!user.IsHoldingItem) return 0f;           // nothing in hand -> nothing to give

        if (!TryDeliver(user.HeldItem))               // wrong item -> whiff, keep holding it
        {
            OnWrongItem?.Invoke(user.HeldItem);
            return 0f;
        }
        user.ConsumeHeldItem();
        return 1f;
    }

    public void StopInteract(ExecuteBehaviour user) { }   // no per-user progress to cancel

    // True if the item belonged on the order; completes + pays out when the order is filled.
    private bool TryDeliver(ItemInfo item)
    {
        if (!_order.TryFulfill(item)) return false;
        OnItemAccepted?.Invoke(item);

        if (_order.IsComplete)
        {
            int tip = Happiness >= tipThreshold ? Mathf.CeilToInt(maxTip * Happiness) : 0;
            OnPaid?.Invoke(_order.TotalPrice(), tip);
            Leave(_order.TotalPrice() + tip);
        }
        return true;
    }

    private void Leave(int payment)
    {
        _done = true;
        if (payment > 0 && GameManager.Instance != null)
            GameManager.Instance.AddMoney(payment);
        Destroy(gameObject, lingerSeconds);   // linger + idle a beat so the popups can deal out before we vanish
    }
}
