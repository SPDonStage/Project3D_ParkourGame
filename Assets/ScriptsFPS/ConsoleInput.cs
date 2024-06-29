using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConsoleInput : InputBase
{
    public override void ReadInput()
    {
        float moveForward = Input.GetAxis("Vertical");
        float moveRight = Input.GetAxis("Horizontal");
        float rotateX = Input.GetAxis("RightStickHorizontal");
        float rotateY = Input.GetAxis("RightStickVertical");

        moveDirection = new Vector3(moveRight, 0, moveForward);
        rotateDirection = new Vector2(rotateX, rotateY);
    }
}
