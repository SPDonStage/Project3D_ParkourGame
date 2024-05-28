using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SageWall : MonoBehaviour
{
    [SerializeField] private float health;
    public Vector3 transformInIt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 3, 3f * Time.deltaTime),transformInIt.z);
    }
}
