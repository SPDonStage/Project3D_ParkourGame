using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public string playerName;
    public int health;
    public int armor;
    public float speed;
    public bool isAlive;

    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;

    protected Vector3 moveDirection = Vector3.zero;
    protected float rotationX = 0;
    protected CharacterController characterController;
    protected Animator animator;

    protected bool canMove = true;
    protected bool isJumping = false;

    //public Transform gunBarrel;
    //public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float fireRate = 0.5f;
    protected float nextFireTime = 0f;

    protected virtual void Start()
    {
        health = 100;
        armor = 50;
        speed = walkSpeed;
        isAlive = true;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TakeDamage(int amount)
    {
        int damageToHealth = amount;
        if (armor > 0)
        {
            int damageToArmor = Mathf.Min(armor, amount);
            armor -= damageToArmor;
            damageToHealth -= damageToArmor;
        }

        health -= damageToHealth;
        if (health <= 0)
        {
            health = 0;
            isAlive = false;
            Die();
        }
    }
    /*    public void Heal(int amount)
        {
            if (isAlive)
            {
                health += amount;
                health = Mathf.Min(health, 100);
            }
        }*/

    protected virtual void Die()
    {
        health = 0;
        isAlive = false;
        Debug.Log(playerName + " has died.");
    }

    protected void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.name == "Ocean" || hit.gameObject.CompareTag("Ocean"))
        {
            Die();
        }

        switch (hit.gameObject.tag)
        {
            case "JumpPad":
                jumpPower = 14f;
                break;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("jumpPad"))
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower);
        }
    }
}
