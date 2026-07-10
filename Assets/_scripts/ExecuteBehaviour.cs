using _scripts;
using UnityEngine;

public class ExecuteBehaviour : MonoBehaviour
{
    public CharacterProperties characterProperties;
    [SerializeField] private LayerMask interactableMask;
    private IIntentSource _intentSource;
    private IInteractable _engaged;
    private bool _handleHeld;

    // read by a loading bar over the character's head
    public float CraftProgress01 { get; private set; }
    public bool IsCrafting { get; private set; }
    
    // the character's single hand slot; null = empty hands
    public ItemInfo HeldItem { get; private set; }
    public bool IsHoldingItem => HeldItem != null;

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
    }

    // called by a customer when a delivery is accepted; empties the hand slot
    public void ConsumeHeldItem()
    {
        HeldItem = null;
    }

    void Handle(bool intent)
    {
        // edge-triggered: pick up / drop is a discrete action, not a continuous hold
        bool pressed = intent && !_handleHeld;
        _handleHeld = intent;
        if (!pressed) return;

        if (IsHoldingItem) Drop();
        else TryPickup();
    }

    void Drop()
    {
        // TODO: place HeldItem on a counter in range, else spawn it on the floor.
        // For now just empty the hand slot so crafting is possible again.
        HeldItem = null;
    }

    void TryPickup()
    {
        // TODO: pick up an item from a counter / floor in range once world items exist.
    }
    
}