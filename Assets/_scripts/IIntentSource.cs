using System;
using UnityEngine;

public struct SourceFrame
{
    public Vector2 moveIntent; // movement intent, normalized vector
    public bool interactIntent; // one button: counter place/pickup, else craft/serve
}

public interface IIntentSource
{
    SourceFrame Sample();   // called once per FixedUpdate
    Vector2 CurrentPosition();
}