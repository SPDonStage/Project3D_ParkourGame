using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).transform.rotation = Quaternion.LookRotation(-dir);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {

        transform.Translate(dir * Time.fixedDeltaTime * speed);
    }
    public void setDir(Vector3 dir)
    {
        this.dir = dir;
    }
    private Vector3 getDir()
    {
        return dir;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            Destroy(gameObject);        
        }
    }
}
