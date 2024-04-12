
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum CharacterState
    {
        Ground,
        Crouch,
        Jump,
        Vault,
    }
    //Layer
    [SerializeField] private LayerMask groundLayer;


    //
    [Header("---Player Setting---")]
    [SerializeField] private CharacterState state;
    [SerializeField] private float speed;
    private CharacterController characterController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    [SerializeField] Camera camera;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform cameraPosition;
    [Header("---Movement---")]
    //Movement
    Vector2 vector2MovementInput;
    Vector3 vector3Movement;
    Vector2 currentSpeed = Vector2.zero;
    //Jump
    [Header("---Jump---")]
    [SerializeField] private float jumpMoveSpeed;
    Vector3 jumpHeightVector3;
    [SerializeField] Transform groundDetector;
    [SerializeField] Vector3 groundDetectorVector3;
    [SerializeField] private float jumpForce;
    //Crouch
    [Header("---Crouch---")]
    //Vault
    [Header("---Vault---")]
    [SerializeField] private List<NewParkourAction> newParkourActions;
    private EnvironmentChecker environmentChecker;
    private bool isInAction = false;
    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();  
        playerInput = new PlayerInput();
        environmentChecker = GetComponent<EnvironmentChecker>();
        camera = Camera.main;
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
        if (isInAction == false)
        {
            transform.rotation = Quaternion.AngleAxis(camera.transform.rotation.eulerAngles.y, Vector3.up);
        }
    }
    private void FixedUpdate()
    {
        
        Movement();
        Crouch();
        //Vault
        if (isInAction == false)
        {
            if (environmentChecker.checkData().Yoffset_Ray_Hit_Check == true)
            {
                if (playerInput.Player.Jump.ReadValue<float>() == 1)
                {

                    foreach (var action in newParkourActions)
                    {
                        if (action.checkIfAvailable(environmentChecker.checkData(), transform))
                        {
                            StartCoroutine(VaultOverObstacle(action));
                            break;
                        }
                    }
                }
            }
            if (state != CharacterState.Vault)
                Jump();
            
        }
    
      
        
    }
    private void SwitchCharacterStateAnimation(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Ground:
                {
                    characterController.center = new Vector3(0, 1.85f, 0);
                    characterController.height = 3.6f;
                    animator.applyRootMotion = true;
                    break;
                }
            case CharacterState.Crouch:
                {
                    characterController.center = new Vector3(0, 1.1f, 0);
                    characterController.height = 2.2f;
                    break;
                }
            case CharacterState.Jump:
                {
                    if (isInAction == false)
                    {
                        animator.CrossFade("On Air", 0.2f);
                        characterController.center = new Vector3(0, 1.1f, 0);
                        characterController.height = 2.2f;
                    }
                    break;
                }
            case CharacterState.Vault:
                {
                    isInAction = true;
                    break;
                }
        }
    }
    void Movement()
    {
        vector2MovementInput = playerInput.Player.Movement.ReadValue<Vector2>();
        vector3Movement = new Vector3(vector2MovementInput.x, 0, vector2MovementInput.y);
        if (vector3Movement.sqrMagnitude >= 0)
        {
            currentSpeed.x = Mathf.Lerp(currentSpeed.x, vector3Movement.x, speed * Time.fixedDeltaTime);
            currentSpeed.y = Mathf.Lerp(currentSpeed.y, vector3Movement.z, speed * Time.fixedDeltaTime);
            animator.SetBool("Sprint", playerInput.Player.Sprint.ReadValue<float>() == 1 ? true : false);
        }
        animator.SetFloat("Movement.x", currentSpeed.x);
        animator.SetFloat("Movement.y", currentSpeed.y);
    }  
    void Jump()
    {
        groundDetectorVector3 = new Vector3(groundDetector.position.x, groundDetector.position.y, groundDetector.position.z);
        Physics.Raycast(groundDetectorVector3, Vector3.down, out var checkGround, 0.3f, groundLayer);
        if (checkGround.collider != null)
        {
            if (state != CharacterState.Crouch)
            {
                SwitchCharacterStateAnimation(state = CharacterState.Ground);
                animator.SetBool("Jump", false);
                jumpHeightVector3.x = Mathf.Lerp(jumpHeightVector3.x, 0, speed * Time.fixedDeltaTime);
                jumpHeightVector3.z = Mathf.Lerp(jumpHeightVector3.z, 0, speed * Time.fixedDeltaTime);
                jumpHeightVector3.y = playerInput.Player.Jump.ReadValue<float>() == 1
                                ? jumpForce : Physics.gravity.y;
            }
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
   
    void Crouch()
    {
        if (playerInput.Player.Crouch.ReadValue<float>() == 1)
        {
            SwitchCharacterStateAnimation(state = CharacterState.Crouch);
            animator.SetBool("isCrouching", true);  
        }
        else
        {
            SwitchCharacterStateAnimation(state = CharacterState.Ground);
            animator.SetBool("isCrouching", false);
        }
    }
    IEnumerator VaultOverObstacle(NewParkourAction newParkourAction)
    {
        SwitchCharacterStateAnimation(state = CharacterState.Vault);
        animator.CrossFade(newParkourAction.AnimationName, 0.2f);
        var animatorState = animator.GetNextAnimatorStateInfo(0);
        float timeCounter = 0;
        while (timeCounter <= animatorState.length)
        {
            if (newParkourAction.IsLookAtObstacle == true)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, newParkourAction.RotatingToObstacle,150 * Time.fixedDeltaTime);          
                virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = Quaternion.RotateTowards(cameraPosition.rotation, newParkourAction.RotatingToObstacle, 150 * Time.fixedDeltaTime).eulerAngles.y;
                virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = Mathf.Lerp(virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value, 0, 5 * Time.fixedDeltaTime); ;
            }
            if (newParkourAction.IsMatching == true)
            {
                TargetMatching(newParkourAction);
            }
            timeCounter += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        //    virtualCamera.GetComponent<CinemachineVirtualCamera>().enabled = true;
        }
        isInAction = false;
    }
    private void TargetMatching(NewParkourAction action)
    {
        animator.MatchTarget(action.MatchPosition, transform.rotation, action.AvatarTarget, new MatchTargetWeightMask(new Vector3(0, 0, 0), 0), action.StartTimeMatching, action.EndTimeMatching);
    }
    //private void OnAnimatorMove()
    //{
     
    //    if (state == CharacterState.Ground)
    //    {
           
    //        velocity = animator.deltaPosition * speed;
            
    //    }
    //}
}
