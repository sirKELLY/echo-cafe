public interface IInteractable
{
    float Interact(ExecuteBehaviour user);     // called each FixedUpdate the user is engaged; returns this user's progress 0..1
    void StopInteract(ExecuteBehaviour user);  // called when the user disengages (leaves / releases)
}
