using UnityEngine;

public class ExecuteBehaviour : MonoBehaviour
{
    public CharacterProperties characterProperties;
    private IIntentSource _intentSource;

    public void fixedUpdate()
    {
        SourceFrame sourceFrame = GetSourceFrame();

    }

    SourceFrame GetSourceFrame()
    {
        return _intentSource.Sample();  
    }

    void ExecuteInput(SourceFrame SourceFrame)
    {
        MoveCharacter(SourceFrame.moveIntent);
    }

    void MoveCharacter(Vector2 moveIntent)
    {
        transform.position += new Vector3(moveIntent.x, 0, moveIntent.y) * (Time.fixedDeltaTime * characterProperties.moveSpeed);
    }

    void Interact()
    {
        // check for interactable objects in range and interact with them
    }

    void Handle()
    {
        // check for objects in range and pick them up or drop them
    }

}