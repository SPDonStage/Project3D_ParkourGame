using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideSuction : MonoBehaviour
{
    public Transform suctionPoint1;
    public Transform suctionPoint2;
    public float suctionForce = 10f;
    public float suctionRadius = 5f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (Vector3.Distance(other.transform.position, suctionPoint1.position) <= suctionRadius)
            {
                Vector3 direction = suctionPoint2.position - rb.transform.position;
                rb.AddForce(direction.normalized * suctionForce);
            }
            else if (Vector3.Distance(other.transform.position, suctionPoint2.position) <= suctionRadius)
            {
                Vector3 direction = suctionPoint1.position - rb.transform.position;
                rb.AddForce(direction.normalized * suctionForce);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(suctionPoint1.position, suctionRadius);
        Gizmos.DrawWireSphere(suctionPoint2.position, suctionRadius);
    }
}
