using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReview : MonoBehaviour
{
    [SerializeField] private GameObject reviewGameObject;
    [SerializeField] private GameObject errorGameObject;
    public bool isCollided;
    // Start is called before the first frame update
    void Start()
    {
        reviewGameObject.SetActive(true);
        isCollided = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCollided)
        {
            Debug.Log("a");
            errorGameObject.SetActive(true);
            reviewGameObject.SetActive(false);
        }
        else
        {
            Debug.Log("b");
            errorGameObject.SetActive(false);
            reviewGameObject.SetActive(true);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider)
        {
            Debug.Log("hit");            
            isCollided = true;
        }
       
       
    }
    private void OnCollisionExit(Collision collision)
    {
        isCollided = false;
    }
}
