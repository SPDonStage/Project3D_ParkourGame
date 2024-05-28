using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float jumpForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController otherCharacterController = other.GetComponent<CharacterController>();
        if (otherCharacterController != null)
        {
            otherCharacterController.Move(Vector3.up * jumpForce * Time.deltaTime);
        }
    }
}
