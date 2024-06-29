using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanLift : MonoBehaviour
{
    public float jumpForce = 50f; 
    //public float decayRate = 5f; 

    private CharacterController controller;
    private float currentJumpForce;
    private bool isInPressureZone = false; 

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller = other.GetComponent<CharacterController>();

            if (controller != null)
            {
                currentJumpForce = jumpForce;
                isInPressureZone = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentJumpForce = 0;
            isInPressureZone = false;
            controller = null;
        }
    }

    private void Update()
    {
        if (isInPressureZone && controller != null)
        {
            Vector3 jumpVector = new Vector3(0, currentJumpForce, 0);
            controller.Move(jumpVector * Time.deltaTime);

            //currentJumpForce = Mathf.Max(currentJumpForce - decayRate * Time.deltaTime, 0);
        }
    }
}
