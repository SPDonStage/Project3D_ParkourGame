using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReview : MonoBehaviour
{
    [SerializeField] private GameObject reviewGameObject;
    [SerializeField] private GameObject errorGameObject;
    public bool isCollided;
    [SerializeField] Transform checkGround;
    public bool isGrounded;
    RaycastHit groundHit;
    [SerializeField] private LayerMask layerMask;
    public float y_Position;
    // Start is called before the first frame update
    void Start()
    {
        reviewGameObject.SetActive(true);
        isCollided = false;
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(checkGround.transform.position, -transform.up, out RaycastHit checkIsGrounded, .05f, layerMask);
        if (checkIsGrounded.collider)
            isGrounded = true;
        if (isCollided || !isGrounded)
        {
            errorGameObject.SetActive(true);
            reviewGameObject.SetActive(false);

        }
        else
        {
            if (isGrounded)
            {
                errorGameObject.SetActive(false);
                reviewGameObject.SetActive(true);
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider)
        {          
            isCollided = true;
        }
       
       
    }
    private void OnCollisionExit(Collision collision)
    {
        isCollided = false;
    }
}
