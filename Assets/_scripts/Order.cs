using System.Collections.Generic;
using _scripts;

// A customer's requested items and how many of each remain unserved.
public class Order
{
    private readonly List<ItemInfo> _requested;
    private readonly List<ItemInfo> _remaining;

    public Order(IEnumerable<ItemInfo> items)
    {
        _requested = new List<ItemInfo>(items);
        _remaining = new List<ItemInfo>(_requested);
    }

    public bool IsComplete => _remaining.Count == 0;

    // read-only views for UI: what they still want / what they originally asked for
    public IReadOnlyList<ItemInfo> Remaining => _remaining;
    public IReadOnlyList<ItemInfo> Requested => _requested;

    // Removes one unit matching 'item'. False if that item isn't (still) on the order.
    public bool TryFulfill(ItemInfo item)
    {
        return _remaining.Remove(item);
    }

    public int TotalPrice()
    {
        int sum = 0;
        foreach (ItemInfo item in _requested) sum += item.price;
        return sum;
    }
}
