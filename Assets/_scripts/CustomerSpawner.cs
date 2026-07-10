using System.Collections.Generic;
using UnityEngine;
using _scripts;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private Customer customerPrefab;
    [SerializeField] private List<Transform> seats = new();   // where customers wait to be served
    [SerializeField] private List<ItemInfo> menu = new();     // items orders are drawn from
    [SerializeField] private float spawnInterval = 6f;
    [SerializeField] private int maxItemsPerOrder = 3;

    private readonly Dictionary<Transform, Customer> _occupied = new();
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < spawnInterval) return;
        _timer = 0f;
        TrySpawn();
    }

    private void TrySpawn()
    {
        Transform seat = FreeSeat();
        if (seat == null || menu.Count == 0) return;

        Customer customer = Instantiate(customerPrefab, seat.position, Quaternion.identity);
        customer.SetOrder(BuildOrder());
        _occupied[seat] = customer;
    }

    // A seat is free if never used or its last customer has left (Destroy -> Unity-null).
    private Transform FreeSeat()
    {
        foreach (Transform seat in seats)
            if (!_occupied.ContainsKey(seat) || _occupied[seat] == null)
                return seat;
        return null;
    }

    private List<ItemInfo> BuildOrder()
    {
        int count = Random.Range(1, maxItemsPerOrder + 1);
        var items = new List<ItemInfo>(count);
        for (int i = 0; i < count; i++)
            items.Add(menu[Random.Range(0, menu.Count)]);
        return items;
    }
}
