using UnityEngine;

[CreateAssetMenu(fileName = "CharacterProperties", menuName = "Scriptable Objects/CharacterProperties")]
public class CharacterProperties : ScriptableObject
{
    public float moveSpeed = 5f;
    public float interactRange = 2f;
    
}
