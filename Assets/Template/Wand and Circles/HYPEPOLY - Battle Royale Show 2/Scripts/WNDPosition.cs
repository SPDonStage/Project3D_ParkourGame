using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WNDPosition : MonoBehaviour
{
    public float speed;
    public float distance = 2.5f;

    private Vector3 startPosition;
    private bool movingRight = true;

    void Start()
    {
        startPosition = transform.position;
        speed = Random.Range(0.5f, 2.0f);
    }

    void Update()
    {
        float offset = transform.position.x - startPosition.x;
        if (Mathf.Abs(offset) >= distance)
        {
            movingRight = !movingRight;
        }
        float moveDirection = movingRight ? 1 : -1;
        transform.position += new Vector3(moveDirection * speed * Time.deltaTime, 0, 0);
    }
}
