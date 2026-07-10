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
    [SerializeField] private GameObject spillPrefab;        // generic floor splat; the spilled item's sprite is stamped on it
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
    public event System.Action<ItemInfo> OnItemDropped;     // set down onto a counter
    public event System.Action<ItemInfo> OnItemSpilled;     // splatted on the floor from a dead-end press (comedy hook)
    public event System.Action OnInteractWhiff;             // pressed interact at nothing, empty-handed

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

    // One button. Holding resolves a press in one shot — place on a counter, serve a customer,
    // else spill on the floor. Empty hands pick up from a counter, else engage a station to craft.
    void Interact(bool intent)
    {
        bool pressed = intent && !_interactHeld;
        _interactHeld = intent;

        if (IsHoldingItem)
        {
            // a full-handed press has to go somewhere; no counter and no customer means it spills
            if (pressed && !TryPlaceOnCounter() && !TryServe()) Spill();
            Disengage();
            return;
        }

        if (pressed && TryPickup())   // took something off a counter
        {
            Disengage();
            return;
        }

        // empty hands: engage the nearest station and craft while the button is held
        IInteractable target = intent ? FindNearest() : null;

        if (pressed && target == null) OnInteractWhiff?.Invoke();

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

    // Nearest thing the interact button would act on — station, customer, OR counter.
    // Read-only: used by the on-screen selection ring. Only the player is ever polled,
    // so echoes pay nothing for this. (Does not reuse FindNearest, which filters to
    // IInteractable and would drop counters.)
    public Transform PeekNearestView()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, characterProperties.interactRange, interactableMask);
        Transform view = null;
        float best = float.MaxValue;

        foreach (var hit in hits)
        {
            bool actionable = hit.TryGetComponent(out IInteractable _) || hit.TryGetComponent(out Counter _);
            if (!actionable) continue;

            float d = ((Vector2)transform.position - (Vector2)hit.transform.position).sqrMagnitude;
            if (d < best) { best = d; view = hit.transform; }
        }

        return view;
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

    // Full hands: hand the item to the nearest customer. True if they took it.
    bool TryServe()
    {
        IInteractable target = FindNearest();
        if (target == null) return false;
        target.Interact(this);      // a customer consumes it; a station refuses (full hands)
        return !IsHoldingItem;      // served iff the item left our hand
    }

    // Item leaves the hand as an uninteractable floor stain and is gone. Used by a dead-end
    // interact press and by the echo loop reset. Safe on empty hands (no-op).
    public void Spill()
    {
        ItemInfo item = HeldItem;
        if (item == null) return;

        if (spillPrefab != null)
        {
            GameObject splat = Instantiate(spillPrefab, transform.position, Quaternion.identity);
            var view = splat.GetComponentInChildren<SpriteRenderer>();
            if (view != null && item.spillSprite != null) view.sprite = item.spillSprite;
        }
        HeldItem = null;            // destroyed: out of play (the ItemInfo asset itself is untouched)
        OnItemSpilled?.Invoke(item);
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