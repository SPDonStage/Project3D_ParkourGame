using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Android.Gradle.Manifest;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBase : MonoBehaviour
{
    [Header("Information")]
    public string playerName;
    public int health = 100;
    public int armor = 100;
    public float speed;
    public bool isAlive;
    

    [Header("PlayerSettings")]
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 8f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public Image HealthImage;
    //public TMPro.TextMeshProUGUI HealthText;
    protected Rigidbody rb;
    protected CharacterController characterController;
    protected bool canMove = true;
    protected bool isJumping = false;
    protected Vector3 moveDirection = Vector3.zero;
    protected float rotationX = 0;
    protected Animator animator;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform gunBarrel;
    public bool canShoot = true;
    public int Ammo;
    public int DefaultAmmo;
    public int MaxAmmo;
    public int PickAmmo;
    public TextMeshProUGUI ReloadText;
    public float reloadTime = 2f;
    public bool isReloading = false;
    public TMPro.TextMeshProUGUI TextAmmo;
    public float bulletSpeed = 500f;

    [Header("GravityChanger")]
    public Vector3 defaultGravity = new Vector3(0, -9.81f, 0);
    public Vector3 highGravity = new Vector3(0, -30f, 0);

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

    public void Heal(int amount)
    {
        if (isAlive)
        {
            health += amount;
            health = Mathf.Min(health, 100);
        }
    }

    protected virtual void Die()
    {
        
        HealthImage.fillAmount -= 1;
        //HealthText.SetText("0%");
        isAlive = false;
        Debug.Log(playerName + " has died.");
        Time.timeScale = 0;
    }

    protected void OnControllerColliderHit(ControllerColliderHit hit)
    {
        /*if (hit.gameObject.name == "Ocean" || hit.gameObject.CompareTag("Ocean"))
        {
            Die();
        }*/

        if(hit.gameObject.CompareTag("JumpPad"))
        {
            jumpPower = 21f;
            Physics.gravity = highGravity;
        }
        else
        {
            jumpPower = 7f;
            Physics.gravity = defaultGravity;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Ocean" || other.gameObject.CompareTag("Ocean"))
        {
            Die();
        }

        if (other.gameObject.CompareTag("JumpPad"))
        {
            jumpPower = 21f;
            Physics.gravity = highGravity;
        }
        else
        {
            jumpPower = 7f;
            Physics.gravity = defaultGravity;
        }
    }
}
