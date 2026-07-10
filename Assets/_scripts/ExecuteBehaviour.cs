using _scripts;
using UnityEngine;

public class ExecuteBehaviour : MonoBehaviour
{
    public CharacterProperties characterProperties;
    [SerializeField] private LayerMask interactableMask;
    private IIntentSource _intentSource;
    private IInteractable _engaged;
    private bool _handleHeld;
    private bool _interactHeld;

    // read by a loading bar over the character's head
    public float CraftProgress01 { get; private set; }
    public bool IsCrafting { get; private set; }
    
    // the character's single hand slot; null = empty hands
    [field: SerializeField] public ItemInfo HeldItem { get; private set; }
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
        Handle(SourceFrame.handleIntent);
    }

    void MoveCharacter(Vector2 moveIntent)
    {
        transform.position += new Vector3(moveIntent.x, moveIntent.y, 0) * (Time.fixedDeltaTime * characterProperties.moveSpeed);
    }

    void Interact(bool intent)
    {
        // engage whatever is nearest; each target owns its own hand rule
        // (a station refuses full hands, a customer requires them)
        IInteractable target = intent ? FindNearest() : null;

        // whiff: a fresh press with nothing in range (once per press, not per held frame)
        if (intent && !_interactHeld && target == null) OnInteractWhiff?.Invoke();
        _interactHeld = intent;

        if (!ReferenceEquals(target, _engaged))
        {
            // left range, switched station, or released interact -> cancel the old one
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

    void Handle(bool intent)
    {
        // edge-triggered: pick up / drop is a discrete action, not a continuous hold
        bool pressed = intent && !_handleHeld;
        _handleHeld = intent;
        if (!pressed) return;

        if (IsHoldingItem) TryPlaceOnCounter();
        else TryPickup();
    }

    // force-discard the held item (used by the echo loop reset); places nothing in the world
    public void Drop()
    {
        if (HeldItem == null) return;

        ItemInfo item = HeldItem;
        HeldItem = null;
        OnItemDropped?.Invoke(item);
    }

    // Handle with empty hands: take whatever is on the nearest counter
    void TryPickup()
    {
        Counter counter = FindNearestCounter();
        if (counter == null || counter.IsEmpty) return;   // nothing to grab -> whiff
        ReceiveItem(counter.Take());
    }

    // Handle with full hands: set the held item down on a free counter
    void TryPlaceOnCounter()
    {
        Counter counter = FindNearestCounter();
        if (counter == null) return;                      // no surface in reach -> keep holding

        ItemInfo item = HeldItem;
        if (counter.TryPlace(item))
        {
            HeldItem = null;
            OnItemDropped?.Invoke(item);
        }
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