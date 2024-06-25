using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float health;
    public bool isRestored; //trigger
    public bool isRestoring;
    [SerializeField] public GameObject shieldGameObject;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRestored)
        {
            if (isRestoring)
            {
                StartCoroutine(healthRestore());
                isRestoring = false;
            }
        }
        else
        {
            StopCoroutine(healthRestore());
        }
    }
    IEnumerator healthRestore()
    {
        while (health < maxHealth)
        {
            Debug.Log(health);
            yield return new WaitForSeconds(1);
            health++;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            Debug.Log("zzzx");
            health--;
        }
    }
}
