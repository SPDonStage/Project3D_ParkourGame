
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
        Slide,
    }
    //Layer
    [SerializeField] private LayerMask groundLayer;


    //
    [Header("---Player Setting---")]
    [SerializeField] private CharacterState state;
    [SerializeField] private float speed;
    [SerializeField] private float speedMouse;
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
    private Vector3 slopeSlideVelocity = Vector3.zero;
    //Crouch
    [Header("---Crouch---")]
    private float slopeSlidingDirectionAngle = 0;
    [SerializeField] private float slopeSlideSpeed = 0;
    private bool isSlopeSliding = false;
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(camera.transform.rotation.eulerAngles.y, Vector3.up), speedMouse * Time.fixedDeltaTime);
        }
    }
    private void FixedUpdate()
    {        
        Movement();
        Crouch();
        Slide();
        SlopeSlide();
        //Vault
        if (isInAction == false)
        {
            if (environmentChecker.checkData().Yoffset_Ray_Hit_Check == true)
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
            if (state != CharacterState.Vault)
                Jump();           
        }
    }
    private void OnDrawGizmos()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit test, 5);
        float angle = Vector3.Angle(test.normal, Vector3.up);
        
        //SetSlopeSlideVelocity(); 
        // Debug.Log("slope:"+ Vector3.ProjectOnPlane(Vector3.down, slopeSlideRay_Hit.normal));
     //   Debug.Log(Vector3.Dot(slopeSlideRay_Hit.normal, transform.forward));
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position,Vector3.up);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, test.normal);
        //   Gizmos.DrawRay(transform.position, test.normal);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position + Vector3.up, Vector3.down);
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
            case CharacterState.Slide:
                {
                    if (isSlopeSliding)
                    {
                     //   animator.applyRootMotion = false;
                        animator.CrossFade("Running Slide Loop", 0.2f);
                    }
                    else
                    {
                        animator.applyRootMotion = true;
                    }
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
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeSlideRay_Hit, 1);
        slopeSlidingDirectionAngle = Vector3.Dot(slopeSlideRay_Hit.normal, transform.forward);
        if ((slopeSlidingDirectionAngle >= 0.0001f || slopeSlidingDirectionAngle <= -0.0001f) && state == CharacterState.Slide) //0.0001 to avoid very small/Epsilon value
        {
            isSlopeSliding = true;
            if (slopeSlidingDirectionAngle < 0) //Upward Direction
            {

                characterController.Move(transform.TransformDirection(vector3Movement) * 0); //stay sliding
            }
            else //Downward Direction
            {
                characterController.Move(transform.TransformDirection(vector3Movement) * slopeSlideSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            Debug.Log("sas");
            isSlopeSliding = false;
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
                virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = Quaternion.RotateTowards   //horizontal rotation
                    (cameraPosition.rotation, newParkourAction.RotatingToObstacle, 150 * Time.fixedDeltaTime).eulerAngles.y;
                virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = Mathf.Lerp
                    (virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value, 0, 5 * Time.fixedDeltaTime); ; //vertical rotation
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
    private void SetSlopeSlideVelocity()
    {
        float angle = 0;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeSlideRay_Hit, 5)) 
        { 
            angle = Vector3.Angle(slopeSlideRay_Hit.normal, Vector3.up); 
            if (angle >= characterController.slopeLimit)
            {
                slopeSlideVelocity = Vector3.ProjectOnPlane(Vector3.down, slopeSlideRay_Hit.normal);
                return;
            } 
        }
        slopeSlideVelocity = Vector3.zero;
    }
    //private void OnAnimatorMove()
    //{
     
    //    if (state == CharacterState.Ground)
    //    {
           
    //        velocity = animator.deltaPosition * speed;
            
    //    }
    //}
}
