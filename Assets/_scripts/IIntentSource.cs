using UnityEngine;

public struct SourceFrame
{
    public Vector2 moveIntent; // movement intent, normalized vector
    public bool interactIntent; // interact with object intent (cooking station, etc)
    public bool handleIntent; // pick up / drop object intent
}

interface IIntentSource
{
    SourceFrame Sample();   // called once per FixedUpdate
}