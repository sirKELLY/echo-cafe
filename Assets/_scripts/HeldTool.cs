using UnityEngine;

namespace _scripts
{
    // A tool that lives at a stand and runs an effect while a character holds it.
    // Its "home" is wherever it starts (parent + position); dropping always returns it there.
    public abstract class HeldTool : MonoBehaviour
    {
        [SerializeField] protected float range = 1.5f;   // reach of the effect while held

        private Transform _homeParent;
        private Vector3 _homePosition;
        private Quaternion _homeRotation;

        public bool IsHeld { get; private set; }

        protected virtual void Awake()
        {
            var t = transform;
            _homeParent = t.parent;
            _homePosition = t.position;
            _homeRotation = t.rotation;
        }

        // Grabbed: sit in the holder's hand.
        public void PickUp(Transform handAnchor)
        {
            IsHeld = true;
            transform.SetParent(handAnchor, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        // Dropped (or any forced release): snap back to the stand.
        public void ReturnHome()
        {
            IsHeld = false;
            transform.SetParent(_homeParent, true);
            transform.SetPositionAndRotation(_homePosition, _homeRotation);
        }

        // Runs each interval while held; holderPosition is where the effect is centered.
        public abstract void TickHeld(Vector2 holderPosition);
    }
}
