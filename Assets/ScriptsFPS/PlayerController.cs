using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class PlayerController : PlayerBase
{
    [Header("Stamina")]
    private float maxStamina = 100f;
    private float currentStamina;
    private float staminaRecoveryRate = 0.1f;
    private float staminaConsumptionRunning = 5f;
    private float staminaConsumptionJumping = 10f;
    private bool isRecoveringStamina = false;
    public Image StaminaImage;

    Vector3 playerVelocity = Vector3.zero;

    bool isRunning;

    protected override void Start()
    {
        base.Start();
        currentStamina = maxStamina;
        UpdateStaminaUI();  // Update UI at start
    }

    void Update()
    {
        if (isAlive)
        {
            Move();
            LookAround();
            //applyGravity();
            //Shoot();
        }
    }

    void Move()
    {
        if (currentStamina <= 0)
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Update animator parameters for walking and running
        animator.SetFloat("isWalking", Mathf.Abs(curSpeedX) + Mathf.Abs(curSpeedY));
        animator.SetBool("isRunning", isRunning);

        if (Input.GetButton("Jump") && canMove)
        {
            isJumping = true;
            animator.SetBool("isJumping", isJumping);
            if (currentStamina > 0)
            {
                moveDirection.y = jumpPower;
                currentStamina = currentStamina - staminaConsumptionJumping * Time.deltaTime;
                UpdateStaminaUI();
            }
        }
        else
        {
            moveDirection.y = movementDirectionY;
            isJumping = false;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        if (isRunning && canMove)
        {
            if (currentStamina > 0)
            {
                currentStamina = currentStamina - staminaConsumptionRunning * Time.deltaTime;

                if (currentStamina < 0)
                {
                    currentStamina = 0;
                }
                UpdateStaminaUI();  // Update UI after consuming stamina
            }
        }

        if (!isRunning && !Input.GetButton("Jump") && characterController.isGrounded)
        {
            if (!isRecoveringStamina)
            {
                StartCoroutine(RecoverStamina());
            }
        }
    }


    IEnumerator RecoverStamina()
    {
        isRecoveringStamina = true;
        while (currentStamina < maxStamina && !isRunning && !Input.GetButton("Jump") && characterController.isGrounded)
        {
            currentStamina = currentStamina + staminaRecoveryRate;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
            UpdateStaminaUI();
            yield return new WaitForSeconds(0.01f);
        }
        isRecoveringStamina = false;
    }

    

    void LookAround()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    /*void Shoot()
    {
        if (Input.GetButtonDown("Fire1") && CurrentEnergy == MaxEnergy && canShoot == true)
        {
            GameObject bullet = Instantiate(bulletPrefab, gunBarrel.transform.position, gunBarrel.rotation);
            //bullet.transform.localPosition = new Vector3((float)-31.593, (float)88.475,(float)13.445);
            //bullet.transform.position = gunBarrel.transform.position;
            //rb.velocity = gunBarrel.forward * bulletSpeed * Time.deltaTime;
            bullet.GetComponent<Rigidbody>().AddForce( bullet.transform.forward * bulletSpeed, ForceMode.Force);
            Destroy(bullet, 5);
            EnergyImage.fillAmount -= 0.25f;
        }

        if (CurrentEnergy == 0 && CurrentEnergy != MaxEnergy)
        {
            StartCoroutine(ShootCooldown());
            canShoot = false;
        }
    }*/

    protected override void Die()
    {
        base.Die();
        Debug.Log(playerName + " has died.");
    }

    void applyGravity()
    {
        playerVelocity += Vector3.down * gravity * Time.fixedDeltaTime;
        characterController.Move(playerVelocity * Time.fixedDeltaTime);
    }

    void UpdateStaminaUI()
    {
        if (StaminaImage != null)
        {
            StaminaImage.fillAmount = currentStamina / maxStamina;
        }
    }
}
