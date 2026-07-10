using UnityEngine;
using UnityEngine.InputSystem;


public class GetPlayerInput : MonoBehaviour, IIntentSource
{
    private InputSystem_Actions _inputSystem;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _inputSystem = new InputSystem_Actions();
        _inputSystem.Enable();
    }
    

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Move Intent: " + _inputSystem.Player.Move.ReadValue<Vector2>());
    }

    public SourceFrame Sample()
    {
        var moveInput = _inputSystem.Player.Move.ReadValue<Vector2>();
        SourceFrame frame = new SourceFrame();
        frame.moveIntent = moveInput;

        // this will pull the input from the player and return it as a SourceFrame
        return frame;
    }
    
    public Vector2 CurrentPosition()
    {
        return transform.position;
    }
}
