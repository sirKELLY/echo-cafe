using UnityEngine;

namespace _scripts
{
    // A staging surface: holds one item as data and renders it. Mirrors the hand slot on
    // ExecuteBehaviour — the item never becomes its own GameObject, the counter just shows it.
    public class Counter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer itemView;      // child renderer that shows the held item
        [SerializeField] private ItemInfo startingItem;        // optional: pre-stock this counter

        [SerializeField] private ItemInfo _item;

        private float _bornTime;   // Time.time the current item was made; freshness ages from here and travels with the item

        public ItemInfo CurrentItem => _item;
        public bool IsEmpty => _item == null;
        public bool HasItem => _item != null;
        public float BornTime => _bornTime;   // read by the picker-upper so freshness survives the pickup

        public event System.Action<ItemInfo> OnItemPlaced;
        public event System.Action<ItemInfo> OnItemTaken;

        private void Awake()
        {
            _item = startingItem;
            _bornTime = Time.time;
            RefreshView();
        }

        // food left sitting eventually goes stale -> tint it green
        private void Update()
        {
            if (itemView == null || _item == null) return;
            if (Time.time - _bornTime >= _item.freshSeconds) itemView.color = Color.green;
        }

        // Set an item down. False if the surface is already occupied. bornTime carries the food's age.
        public bool TryPlace(ItemInfo item, float bornTime)
        {
            if (_item != null) return false;
            _item = item;
            _bornTime = bornTime;
            RefreshView();
            OnItemPlaced?.Invoke(item);
            return true;
        }

        // Take the item off the counter, or null if there's nothing here.
        public ItemInfo Take()
        {
            if (_item == null) return null;
            ItemInfo taken = _item;
            _item = null;
            RefreshView();
            OnItemTaken?.Invoke(taken);
            return taken;
        }

        private void RefreshView()
        {
            if (itemView == null) return;
            itemView.sprite = _item != null ? _item.sprite : null;
            itemView.enabled = _item != null;
            itemView.color = Color.white;   // fresh until Update decides it's stale
        }
    }
}
