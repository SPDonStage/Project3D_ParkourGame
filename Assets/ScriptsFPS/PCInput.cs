using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PCInput : InputBase
{
    public override void ReadInput()
    {
        float moveForward = Input.GetAxis("Vertical");
        float moveRight = Input.GetAxis("Horizontal");
        float rotateX = Input.GetAxis("Mouse X");
        float rotateY = Input.GetAxis("Mouse Y");

        moveDirection = new Vector3(moveRight, 0, moveForward);
        rotateDirection = new Vector2(rotateX, rotateY);
    }
}
