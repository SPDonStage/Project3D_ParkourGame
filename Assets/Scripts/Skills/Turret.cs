using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private TurretBullet turretBullet;
    [SerializeField] private Transform firePos;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject head;
    [SerializeField] private int shootCD;
    [SerializeField] private GameObject alarm;
    [SerializeField] private Material noneMaterial;
    [SerializeField] private Material detectedMaterial;
    private bool isCanShoot = false;
    private Vector3 dir;
    // Start is called before the first frame update
    void Awake()
    {
        alarm.GetComponent<MeshRenderer>().material = noneMaterial;
    }
    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {           
            alarm.GetComponent<MeshRenderer>().material = detectedMaterial;
            dir = other.gameObject.transform.position - head.transform.position;
            head.transform.rotation = Quaternion.LookRotation(-dir);
           
            if (isCanShoot) 
                StartCoroutine(CanShoot(shootCD));
        }       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCanShoot = true;
        }
    }  
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCanShoot = false;
            StopAllCoroutines();
            head.transform.rotation = new Quaternion(0, 0, 0, 0);
            alarm.GetComponent<MeshRenderer>().material = noneMaterial;
        }
    }
    IEnumerator CanShoot(int time)
    {
        isCanShoot = false;
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
        }
        if (time == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                Instantiate(turretBullet, firePos.position, Quaternion.identity).setDir(dir);
                yield return new WaitForSeconds(0.5f);
            }
            time = shootCD;
            isCanShoot = true;
        }
        
    }
}
