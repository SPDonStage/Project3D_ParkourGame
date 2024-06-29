using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : InputBase
{
    public override void ReadInput()
    {
        moveDirection = new Vector3(Input.acceleration.x, 0, Input.acceleration.y);
        rotateDirection = Vector2.zero;
    }
}
