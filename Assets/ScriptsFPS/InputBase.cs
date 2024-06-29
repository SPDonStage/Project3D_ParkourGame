using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputBase : MonoBehaviour
{
    public Vector3 moveDirection { get; protected set; }
    public Vector2 rotateDirection { get; protected set; }

    protected virtual void Start()
    {
        moveDirection = Vector3.zero;
        rotateDirection = Vector2.zero;
    }

    public abstract void ReadInput();
}
