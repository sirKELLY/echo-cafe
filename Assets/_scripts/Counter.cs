using UnityEngine;

namespace _scripts
{
    // A staging surface: holds one item as data and renders it. Mirrors the hand slot on
    // ExecuteBehaviour — the item never becomes its own GameObject, the counter just shows it.
    public class Counter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer itemView;      // child renderer that shows the held item
        [SerializeField] private ItemInfo startingItem;        // optional: pre-stock this counter

        private ItemInfo _item;

        public ItemInfo CurrentItem => _item;
        public bool IsEmpty => _item == null;
        public bool HasItem => _item != null;

        public event System.Action<ItemInfo> OnItemPlaced;
        public event System.Action<ItemInfo> OnItemTaken;

        private void Awake()
        {
            _item = startingItem;
            RefreshView();
        }

        // Set an item down. False if the surface is already occupied.
        public bool TryPlace(ItemInfo item)
        {
            if (_item != null) return false;
            _item = item;
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
        }
    }
}
