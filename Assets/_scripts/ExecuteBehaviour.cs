using _scripts;
using UnityEngine;

public class ExecuteBehaviour : MonoBehaviour
{
    public CharacterProperties characterProperties;
    [SerializeField] private LayerMask interactableMask;
    private IIntentSource _intentSource;
    private IInteractable _engaged;
    private bool _interactHeld;

    // read by a loading bar over the character's head
    public float CraftProgress01 { get; private set; }
    public bool IsCrafting { get; private set; }
    
    // the character's single hand slot; null = empty hands
    [SerializeField] private SpriteRenderer heldItemView;   // child renderer that shows the carried item
    [SerializeField] private ItemInfo _heldItem;
    public ItemInfo HeldItem
    {
        get => _heldItem;
        private set { _heldItem = value; RefreshHandView(); }   // set once, the sprite always follows
    }
    public bool IsHoldingItem => HeldItem != null;

    // what we're engaged with (highlight it) and where we're headed (animator / facing / footsteps)
    public IInteractable EngagedTarget => _engaged;
    public Vector2 MoveIntent { get; private set; }

    // discrete hand moments — fire identically for the player and every echo
    public event System.Action<ItemInfo> OnItemReceived;    // craft finished into hand
    public event System.Action<ItemInfo> OnItemDelivered;   // a customer took it
    public event System.Action<ItemInfo> OnItemDropped;     // discarded (incl. echo loop reset)
    public event System.Action OnInteractWhiff;             // pressed interact at nothing (comedy hook)

    void Awake()
    {
        Debug.Log("ExecuteBehaviour Awake");
        _intentSource = GetComponent<IIntentSource>();
        Debug.Assert(_intentSource != null, "No IIntentSource found on this GameObject. Please add one.");
        RefreshHandView();   // sync the sprite to whatever the slot deserialized with
    }

    public void FixedUpdate()
    {
        SourceFrame sourceFrame = GetSourceFrame();
        ExecuteInput(sourceFrame);
    }

    SourceFrame GetSourceFrame()
    {
        SourceFrame sourceFrame = _intentSource.Sample(); 
        return  sourceFrame;
    }

    void ExecuteInput(SourceFrame SourceFrame)
    {
        MoveIntent = SourceFrame.moveIntent;
        MoveCharacter(SourceFrame.moveIntent);
        Interact(SourceFrame.interactIntent);
    }

    void MoveCharacter(Vector2 moveIntent)
    {
        transform.position += new Vector3(moveIntent.x, moveIntent.y, 0) * (Time.fixedDeltaTime * characterProperties.moveSpeed);
    }

    // One button. A fresh press first tries a counter hand-off (place if holding, pick up if
    // empty hands); only if that does nothing does it fall through to continuous craft/serve.
    void Interact(bool intent)
    {
        bool pressed = intent && !_interactHeld;

        if (pressed)
        {
            bool handled = IsHoldingItem ? TryPlaceOnCounter() : TryPickup();
            if (handled)
            {
                _interactHeld = true;
                Disengage();          // a counter hand-off consumes the press; don't also craft
                return;
            }
        }

        // fall-through: engage whatever is nearest; each target owns its own hand rule
        // (a station refuses full hands, a customer requires them)
        IInteractable target = intent ? FindNearest() : null;

        // whiff: a fresh press that did nothing at all — no counter, no interactable
        if (intent && !_interactHeld && target == null) OnInteractWhiff?.Invoke();
        _interactHeld = intent;

        if (!ReferenceEquals(target, _engaged))
        {
            // left range, switched target, or released -> cancel the old one
            _engaged?.StopInteract(this);
            _engaged = target;
        }

        if (_engaged != null)
        {
            CraftProgress01 = _engaged.Interact(this);
            IsCrafting = true;
        }
        else
        {
            CraftProgress01 = 0f;
            IsCrafting = false;
        }
    }

    void Disengage()
    {
        _engaged?.StopInteract(this);
        _engaged = null;
        CraftProgress01 = 0f;
        IsCrafting = false;
    }

    IInteractable FindNearest()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, characterProperties.interactRange, interactableMask);
        IInteractable nearest = null;
        float best = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IInteractable interactable)) continue;

            float d = ((Vector2)transform.position - (Vector2)hit.transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                nearest = interactable;
            }
        }

        return nearest;
    }

    // called by the station when a craft completes; the crafter always has empty hands here
    public void ReceiveItem(ItemInfo item)
    {
        HeldItem = item;
        OnItemReceived?.Invoke(item);
    }

    // called by a customer when a delivery is accepted; empties the hand slot
    public void ConsumeHeldItem()
    {
        ItemInfo item = HeldItem;
        HeldItem = null;
        OnItemDelivered?.Invoke(item);
    }

    // mirror the hand slot onto the character's item sprite (empty hands hide it)
    void RefreshHandView()
    {
        if (heldItemView == null) return;
        heldItemView.sprite = _heldItem != null ? _heldItem.sprite : null;
        heldItemView.enabled = _heldItem != null;
    }

    // force-discard the held item (used by the echo loop reset); places nothing in the world
    public void Drop()
    {
        if (HeldItem == null) return;

        ItemInfo item = HeldItem;
        HeldItem = null;
        OnItemDropped?.Invoke(item);
    }

    // Empty hands: take whatever is on the nearest counter. True if something was taken.
    bool TryPickup()
    {
        Counter counter = FindNearestCounter();
        if (counter == null || counter.IsEmpty) return false;
        ReceiveItem(counter.Take());
        return true;
    }

    // Full hands: set the held item down on a free counter. True if it was placed.
    bool TryPlaceOnCounter()
    {
        Counter counter = FindNearestCounter();
        if (counter == null) return false;                // no surface in reach -> keep holding

        ItemInfo item = HeldItem;
        if (!counter.TryPlace(item)) return false;        // occupied -> keep holding

        HeldItem = null;
        OnItemDropped?.Invoke(item);
        return true;
    }

    Counter FindNearestCounter()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, characterProperties.interactRange, interactableMask);
        Counter nearest = null;
        float best = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out Counter counter)) continue;

            float d = ((Vector2)transform.position - (Vector2)hit.transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                nearest = counter;
            }
        }

        return nearest;
    }
    
}