using UnityEngine;

[CreateAssetMenu(fileName = "CharacterProperties", menuName = "Scriptable Objects/CharacterProperties")]
public class CharacterProperties : ScriptableObject
{
    public float moveSpeed = 5f;
    public float interactRange = 2f;
    public float placeRange = 3.5f;   // wider reach for setting an item on a free counter (see ExecuteBehaviour.TryPlaceOnCounter)

}
