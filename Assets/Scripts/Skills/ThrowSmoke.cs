using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowSmoke : MonoBehaviour
{
    public PlayerController playerController;
    private Rigidbody rb;
    private float xDragSpeed = 10;
    [SerializeField] private float zDragSpeed = 10;
    bool isCollided = false;
    private void Awake()
    {
        rb ??= GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(3, 3, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCollided)
        {
            if (playerController.playerInput.Player.Skill4.IsPressed())
            {               
                rb.AddForce(playerController.camera.transform.forward * zDragSpeed * Time.deltaTime, ForceMode.Impulse);
            }
           
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isCollided = true;
            rb.isKinematic = true;
            transform.localScale = new Vector3(10, 10, 10);
            if (isCollided)
                StartCoroutine(Destroy());
        }
    }
    IEnumerator Destroy()
    {
        int i =0;   
        while (i < 5)
        {           
            yield return new WaitForSeconds(1);
            i++;
        }
        if (i == 4)
            Destroy(this.gameObject);
        isCollided = false;
    }
}
