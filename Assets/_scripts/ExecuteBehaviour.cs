using System;
using _scripts;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class ExecuteBehaviour : MonoBehaviour
{
    public CharacterProperties characterProperties;
    private IIntentSource _intentSource;

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
    }

    void MoveCharacter(Vector2 moveIntent)
    {
        transform.position += new Vector3(moveIntent.x, moveIntent.y, 0) * (Time.fixedDeltaTime * characterProperties.moveSpeed);
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