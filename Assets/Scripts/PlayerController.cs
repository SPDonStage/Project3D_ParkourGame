
using Cinemachine;
using NUnit.Framework.Internal;
using System;
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
        Slide,
        WallRunning,
    }
    //Layer
    [SerializeField] private LayerMask groundLayer;

    
    //
    [Header("---Player Setting---")]
    #region
    [SerializeField] private CharacterState state;
    [SerializeField] private float speed;
    [SerializeField] private float speedMouse;
    private CharacterController characterController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    [SerializeField] Camera camera;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform cameraPosition;
    #endregion
    
    [Header("---Movement---")]
    #region
    //Movement
    Vector2 vector2MovementInput;
    Vector3 vector3Movement;
    Vector2 currentSpeed = Vector2.zero;
    #endregion

    //Jump
    [Header("---Jump---")]
    #region
  //  private float gravity = Physics.gravity.y;
    [SerializeField] private float jumpMoveSpeed;
    Vector3 jumpHeightVector3;
    [SerializeField] Transform groundDetector;
    [SerializeField] Vector3 groundDetectorVector3;
    [SerializeField] private float jumpForce;
    #endregion
    
    //Crouch
    [Header("---Crouch---")]
    #region
    private float slopeSlidingDirectionAngle = 0;
    [SerializeField] private float slopeSlidingSpeedValueInput = 0;
    public float slopeSlideSpeed;
    private bool isSlopeSliding = false;
    float slopeAngle = 0;
    private RaycastHit antiStandingOnSlope_Ray;
    #endregion
    
    //Vault
    [Header("---Vault---")]
    #region
    [SerializeField] private List<NewParkourAction> newParkourActions;
    private EnvironmentChecker environmentChecker;
    private bool isInAction = false;
    #endregion
    //Wall Running
    [Header("---Wall Running---")]
    #region
    private bool canWallRunning = false;
    private Vector3 wallRunningVector3;
    [SerializeField] private LayerMask wallRunningLayerMask;
    #endregion

    // Start is called before the first frame update
    private void Awake()
    {
        animator ??= GetComponent<Animator>();
        characterController ??= GetComponent<CharacterController>();  
        playerInput ??= new PlayerInput();
        environmentChecker ??= GetComponent<EnvironmentChecker>();
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(camera.transform.rotation.eulerAngles.y, Vector3.up), speedMouse * Time.fixedDeltaTime);
        }
    }
    private void FixedUpdate()
    {
        Movement();
        Crouch();
        Slide();
        SlopeSlide();
        AntiStandingOnSlope();
        WallRun();
        if (!isInAction)
        {
        
         //   if (state == CharacterState.Jump && canWallRunning)
          //  {
          //      playerInput.Player.Jump.performed += WallRun;
          //  }
        }
        //Vault
        if (!isInAction)
        {
            if (environmentChecker.checkData().Yoffset_Ray_Hit_Check)
            {
                if (playerInput.Player.Jump.ReadValue<float>() == 1)
                {

                    foreach (var action in newParkourActions) 
                    {
                        if (action.checkIfAvailable(environmentChecker.checkData(), transform)) //check which action available
                        { 
                            StartCoroutine(VaultOverObstacle(action));
                            break;
                        }
                    }
                }
            }
            if (state != CharacterState.Vault && state != CharacterState.WallRunning)
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
                   // slopeSlideSpeed = 0;

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
            case CharacterState.Slide:
                {
                    if (!isSlopeSliding && slopeAngle != 0)
                    {
                        slopeSlideSpeed = slopeSlidingSpeedValueInput;
                    }
             
                    break;
                }
            case CharacterState.WallRunning:
                {
                    animator.applyRootMotion = true;
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
        if (checkGround.collider)
        {
            if (playerInput.Player.Crouch.ReadValue<float>() == 0)
            {
                SwitchCharacterStateAnimation(state = CharacterState.Ground);
                animator.SetBool("Jump", false);    
                jumpHeightVector3.x = Mathf.Lerp(jumpHeightVector3.x, 0, speed * Time.fixedDeltaTime);
                jumpHeightVector3.z = Mathf.Lerp(jumpHeightVector3.z, 0, speed * Time.fixedDeltaTime);
           //     gravity = playerInput.Player.Jump.ReadValue<float>() == 1 ? Physics.gravity.y : 0;
                jumpHeightVector3.y = playerInput.Player.Jump.ReadValue<float>() == 1 //set jumpforce
                                ? jumpForce : Physics.gravity.y;
            }           
        }
        else
        {
            SwitchCharacterStateAnimation(state = CharacterState.Jump);
            jumpHeightVector3.x = Mathf.Clamp(jumpHeightVector3.x, -5.5f, 5.5f);
            jumpHeightVector3.z = Mathf.Clamp(jumpHeightVector3.z, -5.5f, 5.5f);
            jumpHeightVector3 += vector3Movement * jumpMoveSpeed * Time.fixedDeltaTime;   //movement on air;
        }        
        jumpHeightVector3.y += Physics.gravity.y * Time.fixedDeltaTime;
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
            animator.SetBool("isCrouching", false);
        }     
    }
    void Slide()
    {
        if (playerInput.Player.Crouch.ReadValue<float>() == 1 && playerInput.Player.Sprint.ReadValue<float>() == 1 && vector3Movement.z > 0)
        {
            SwitchCharacterStateAnimation(state = CharacterState.Slide);
            animator.SetBool("isSliding", true);
        }
        else
        {
            animator.SetBool("isSliding", false);
        }
    }
    void SlopeSlide() //use character controller movement for slope sliding
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeSlideRay_Hit, .3f);
        antiStandingOnSlope_Ray = slopeSlideRay_Hit;
        slopeSlidingDirectionAngle = Vector3.Dot(slopeSlideRay_Hit.normal, transform.forward);
        slopeAngle = 90 - Vector3.Angle(slopeSlideRay_Hit.normal, Vector3.up);
        if (state == CharacterState.Slide) {
            if (slopeSlidingDirectionAngle >= 0.0001f || slopeSlidingDirectionAngle <= -0.0001f) //0.0001 to avoid very small/Epsilon value 
                                                                                                 //check if on slope
            {
                isSlopeSliding = true;
                if (slopeSlidingDirectionAngle < 0) //Upward Direction
                {
                    slopeSlideSpeed = Mathf.Lerp(slopeSlideSpeed, 0, slopeSlideSpeed / 2 * Time.fixedDeltaTime);
                    animator.applyRootMotion = false;
                    animator.CrossFade("Running Slide Upward", 0.2f);
                }
                else //Downward Direction
                {
                    animator.CrossFade("Running Slide Loop", 0.2f); 
                    slopeSlideSpeed = Mathf.Lerp(slopeSlideSpeed, slopeSlidingSpeedValueInput + slopeAngle / 10, slopeSlideSpeed * Time.fixedDeltaTime); 
                }
            }
            else //gradually slow down speed when end slope sliding
            {
                slopeSlideSpeed = Mathf.Lerp(slopeSlideSpeed, 0, slopeSlideSpeed * Time.fixedDeltaTime); 
            }
            if (slopeSlideSpeed <= 1f)
            {
                isSlopeSliding = false;
                slopeAngle = 0;
            }
            characterController.Move(transform.TransformDirection(vector3Movement) * slopeSlideSpeed * Time.fixedDeltaTime);
        }
    }
    void AntiStandingOnSlope() //avoid bhop on slope
    {
        if (slopeAngle < characterController.slopeLimit)
        {
            characterController.Move(new Vector3(antiStandingOnSlope_Ray.normal.x, -antiStandingOnSlope_Ray.normal.y, antiStandingOnSlope_Ray.normal.z) * 5 * Time.fixedDeltaTime);
            SwitchCharacterStateAnimation(state = CharacterState.Jump);
        }
        else
        {

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
            if (newParkourAction.IsLookAtObstacle)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, newParkourAction.RotatingToObstacle,150 * Time.fixedDeltaTime);   //player rotation
            //    virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = Quaternion.RotateTowards   //horizontal rotation
             //       (cameraPosition.rotation, newParkourAction.RotatingToObstacle, 150 * Time.fixedDeltaTime).eulerAngles.y;
              //  virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = Mathf.Lerp
             //       (virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value, 0, 5 * Time.fixedDeltaTime); ; //vertical rotation
            }
            if (newParkourAction.IsMatching)
            {
                TargetMatching(newParkourAction);
            }
            timeCounter += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        //    virtualCamera.GetComponent<CinemachineVirtualCamera>().enabled = true;
        }
        SwitchCharacterStateAnimation(state = CharacterState.Ground);
        isInAction = false;
    }
    private void TargetMatching(NewParkourAction action)
    {
        animator.MatchTarget(action.MatchPosition, transform.rotation, action.AvatarTarget, new MatchTargetWeightMask(new Vector3(0, 0, 0), 0), action.StartTimeMatching, action.EndTimeMatching);
    }
    private void WallRun()
    {
        SwitchCharacterStateAnimation(state = CharacterState.WallRunning);
      //  animator.SetBool("isWallRunning", true);
        Physics.Raycast(transform.position, transform.right, out RaycastHit checkRight_Ray, 1f, wallRunningLayerMask);
        Physics.Raycast(transform.position, -transform.right, out RaycastHit checkLeft_Ray, 1f, wallRunningLayerMask);
        if (checkRight_Ray.collider)
        {
            wallRunningVector3 = Vector3.Cross(checkRight_Ray.normal, Vector3.up);
            Debug.Log(wallRunningVector3);
            characterController.Move(transform.TransformDirection(wallRunningVector3) * Time.fixedDeltaTime);
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if (hit.collider.CompareTag("Running Wall"))
        //{
        // //   Debug.Log("t");
        //    canWallRunning = true;
        //    sideOfWall_Vector3 = transform.InverseTransformPoint(hit.transform.position);
        //    if (sideOfWall_Vector3.x < 0) //left side
        //    {

        //            animator.MatchTarget(hit.point, transform.rotation, AvatarTarget.RightFoot, new MatchTargetWeightMask(new Vector3(0, 0, 0), 0), 0, 1);
        //         //   animator.MatchTarget(hit.point, transform.rotation, AvatarTarget.LeftFoot, new MatchTargetWeightMask(new Vector3(0, 0, 0), 0), 0, 1);
                
        //    }
        //    else //right side
        //    {
               
        //    }
            
        //}
        //else
        //{
        // //   Debug.Log("f");
        //    if (animator.GetBool("isWallRunning"))
        //    {
        //      //  SwitchCharacterStateAnimation(state = CharacterState.Jump);
        //    //    canWallRunning = false;
        //      //  animator.SetBool("isWallRunning", false);
        //    }
            
        //}
    }
    //private void OnAnimatorMove()
    //{

    //    if (state == CharacterState.Ground)
    //    {

    //        velocity = animator.deltaPosition * speed;

    //    }
    //}
}
