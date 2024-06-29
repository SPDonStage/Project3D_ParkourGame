using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBoard : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 5f;
    public float maxRotation = 15f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (rb != null)
            {
                Debug.Log("PLAYER");
                RotateSeesaw();
            }
        }
    }

    void RotateSeesaw()
    {
        // Calculate direction based on player position relative to seesaw
        float direction = player.position.z - transform.position.z;

        // Determine target Z rotation based on player's position
        // Ensure that the direction is significantly large to avoid unnecessary flips
        if (Mathf.Abs(direction) > 0.1f)
        {
            float targetZ = Mathf.Clamp(direction * rotationSpeed, -maxRotation, maxRotation);

            // Create a target rotation Quaternion
            Quaternion targetRotation = Quaternion.Euler(0, 90, targetZ);

            // Smoothly interpolate to the target rotation
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
        }
    }
}