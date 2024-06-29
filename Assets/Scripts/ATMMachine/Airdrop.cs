using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airdrop : MonoBehaviour
{
    [SerializeField] Animator animator;
    private bool isGround;
    [SerializeField] LayerMask layerMask;
    private void Awake()
    {
        animator ??= GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(transform.position, -transform.up, out RaycastHit hitGround, 5, layerMask);
        if (hitGround.collider)
        {
            animator.enabled = true;
        }
    }
}
