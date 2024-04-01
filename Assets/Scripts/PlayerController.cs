
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum CharacterState
    {
        Ground,
        Jump,
    }
    //Layer
    [SerializeField] private LayerMask groundLayer;


    //
    [SerializeField] private CharacterState state;
    [SerializeField] private float speed;
    private CharacterController characterController;
    private EnvironmentChecker environmentChecker;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    
    //Movement
    Vector2 vector2MovementInput;
    Vector3 vector3Movement;
    Vector2 currentSpeed = Vector2.zero;
    //Jump
    public bool isJump = false;
    [SerializeField] private float jumpMoveSpeed;
    Vector3 jumpHeightVector3;
    [SerializeField] Transform groundDetector;
    Vector3 groundDetectorVector3;
    [SerializeField] private float jumpForce;
    
    //Vault
    private bool isInAction = false;
    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();  
        playerInput = new PlayerInput();
        environmentChecker = GetComponent<EnvironmentChecker>();
    }
    private void OnDisable()
    {
        playerInput.Disable();
    }
    private void OnEnable()
    {
        playerInput.Enable();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        state = CharacterState.Ground;
        SwitchCharacterStateAnimation(state);
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.AngleAxis(virtualCamera.transform.rotation.eulerAngles.y, Vector3.up);
    }
    private void FixedUpdate()
    {
        
        Movement();
        if (playerInput.Player.Jump.ReadValue<float>() == 1 && isInAction == false)
        {
            if (environmentChecker.checkData().Yoffset_Ray_Hit_Check == true)
            {
                StartCoroutine(VaultOverObstacle());
            }
        }
        if (environmentChecker.checkData().Yoffset_Ray_Hit_Check == false)
        {

            Jump();
        }

    }
    void Movement()
    {
        vector2MovementInput = playerInput.Player.Movement.ReadValue<Vector2>();
        vector3Movement = new Vector3(vector2MovementInput.x, 0, vector2MovementInput.y);
        if (vector3Movement.sqrMagnitude < 0)
        {

        }
        else
        {
            currentSpeed.x = Mathf.Lerp(currentSpeed.x, vector3Movement.x, speed * Time.fixedDeltaTime);
            currentSpeed.y = Mathf.Lerp(currentSpeed.y, vector3Movement.z, speed * Time.fixedDeltaTime);
            animator.SetBool("Sprint", playerInput.Player.Sprint.ReadValue<float>() == 1 ? true : false);
        }
   
        animator.SetFloat("Movement.x", currentSpeed.x);
        animator.SetFloat("Movement.y", currentSpeed.y);
       

    }
    
    private void SwitchCharacterStateAnimation(CharacterState state)
    {
        switch(state)
        {
            case CharacterState.Ground:
                {
                    animator.applyRootMotion = true;

                    break;
                }          
            case CharacterState.Jump:
                {
                    animator.applyRootMotion = false;
                    animator.CrossFade("On Air", 0.2f);
                    break;
                }
        }
    }
    void Jump()
    {

    
        groundDetectorVector3 = new Vector3(groundDetector.position.x, groundDetector.position.y, groundDetector.position.z);
        Physics.Raycast(groundDetectorVector3, Vector3.down, out var checkGround, 0.1f, groundLayer);
        if (checkGround.collider != null)
        {
            SwitchCharacterStateAnimation(state = CharacterState.Ground);
            animator.SetBool("Jump", false);
            jumpHeightVector3.x = Mathf.Lerp(jumpHeightVector3.x, 0, speed * Time.fixedDeltaTime); 
            jumpHeightVector3.z = Mathf.Lerp(jumpHeightVector3.z, 0, speed * Time.fixedDeltaTime);
            jumpHeightVector3.y = playerInput.Player.Jump.ReadValue<float>() == 1
                            ? jumpForce : Physics.gravity.y;

        }
        else
        {                    
            SwitchCharacterStateAnimation(state = CharacterState.Jump);
            jumpHeightVector3.x = Mathf.Clamp(jumpHeightVector3.x, -5.5f, 5.5f);
            jumpHeightVector3.z = Mathf.Clamp(jumpHeightVector3.z, -5.5f, 5.5f);
            jumpHeightVector3 += vector3Movement * jumpMoveSpeed * Time.fixedDeltaTime;  
        }

        jumpHeightVector3 += Physics.gravity * Time.fixedDeltaTime;
        characterController.Move(transform.TransformDirection(jumpHeightVector3) * Time.fixedDeltaTime);

    }

    IEnumerator VaultOverObstacle()
    {
        isInAction = true;
        animator.CrossFade("Vault Over Obstacle", 0.2f);
        var animatorState = animator.GetNextAnimatorStateInfo(0);
        yield return new WaitForSeconds(animatorState.length);
        isInAction = false;
    }
    private void OnDrawGizmos()
    {
   
 
    }
    //private void OnAnimatorMove()
    //{
     
    //    if (state == CharacterState.Ground)
    //    {
           
    //        velocity = animator.deltaPosition * speed;
            
    //    }
    //}
}
