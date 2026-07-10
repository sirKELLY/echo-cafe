using System.Collections.Generic;
using UnityEngine;
using _scripts;

public class Customer : MonoBehaviour, IInteractable
{
    [Header("Patience / happiness (balance levers)")]
    [SerializeField] private float patienceSeconds = 20f;                 // time until happiness hits 0
    [SerializeField, Range(0f, 1f)] private float tipThreshold = 0.5f;    // happiness above this -> tip
    [SerializeField] private int maxTip = 5;

    private Order _order;
    private float _waitTimer;
    private bool _done;   // paid or left: ignore further deliveries

    // 1 at arrival, 0 once patience is spent.
    public float Happiness => Mathf.Clamp01(1f - _waitTimer / patienceSeconds);

    public void SetOrder(IEnumerable<ItemInfo> items)
    {
        _order = new Order(items);
    }

    private void Update()
    {
        if (_done) return;

        _waitTimer += Time.deltaTime;
        if (_waitTimer >= patienceSeconds) Leave(0);   // patience out -> lost sale
    }

    // IInteractable: same "walk up and hold interact" verb as a WorkStation.
    // Hand-over is instant, so this returns 1 on a successful delivery, 0 otherwise.
    public float Interact(ExecuteBehaviour user)
    {
        if (_done || _order == null) return 0f;
        if (!user.IsHoldingItem) return 0f;           // nothing in hand -> nothing to give

        if (!TryDeliver(user.HeldItem)) return 0f;    // wrong item -> whiff, keep holding it
        user.ConsumeHeldItem();
        return 1f;
    }

    public void StopInteract(ExecuteBehaviour user) { }   // no per-user progress to cancel

    // True if the item belonged on the order; completes + pays out when the order is filled.
    private bool TryDeliver(ItemInfo item)
    {
        if (!_order.TryFulfill(item)) return false;

        if (_order.IsComplete)
        {
            int tip = Happiness >= tipThreshold ? Mathf.CeilToInt(maxTip * Happiness) : 0;
            Leave(_order.TotalPrice() + tip);
        }
        return true;
    }

    private void Leave(int payment)
    {
        _done = true;
        if (payment > 0 && GameManager.Instance != null)
            GameManager.Instance.AddMoney(payment);
        Destroy(gameObject);
    }
}
