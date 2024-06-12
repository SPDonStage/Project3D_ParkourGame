using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    Animator animator;
    bool ray;
    RaycastHit rayHit;
    [SerializeField] float offsetFoot;
    [SerializeField] float offsetWallRun;
    [SerializeField] float offsetHand;
    [SerializeField] LayerMask layerMask;
    public bool isWallRun;
    private Vector3 leftFoot;
    private Vector3 rightFoot;
    Vector3 direction;
    private void Awake()
    {
        animator ??= GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      //  Debug.Log(direction);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            //Left Foot
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            ray = Physics.Raycast(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down, out rayHit,1 + offsetFoot, layerMask);       
            if (ray)
            {
                if (rayHit.collider)
                {
                    Vector3 footPosition = rayHit.point;
                    footPosition.y += offsetFoot;
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, rayHit.normal));
                }
            }           
            //Right Foot
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            ray = Physics.Raycast(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down, out rayHit,1 + offsetFoot, layerMask);         
            if (ray)
            {
                if (rayHit.collider)
                {
                    Vector3 footPosition = rayHit.point;
                    footPosition.y += offsetFoot;
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rayHit.normal));
                }
            }

            if (isWallRun)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                ray = Physics.Raycast(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + transform.TransformDirection(leftFoot), transform.TransformDirection(rightFoot), out rayHit, 1 + offsetFoot, layerMask); 
                Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + transform.TransformDirection(leftFoot), transform.TransformDirection(rightFoot),Color.green,2);
                if (ray)
                {
                    if (rayHit.collider)
                    {
                        Vector3 footPosition = rayHit.point;
                        footPosition.x += offsetWallRun;
                        animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                        animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, rayHit.normal));
                    }
                }

                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                ray = Physics.Raycast(animator.GetIKPosition(AvatarIKGoal.RightFoot) + transform.TransformDirection(leftFoot), transform.TransformDirection(rightFoot), out rayHit, 1 + offsetFoot, layerMask); 
                Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.RightFoot) + transform.TransformDirection(leftFoot), transform.TransformDirection(rightFoot), Color.red, 2);
                if (ray)
                {
                    if (rayHit.collider)
                    {
                        Vector3 footPosition = rayHit.point;
                        footPosition.y += offsetWallRun;
                        animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                        animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rayHit.normal));
                    }
                }
            }

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            ray = Physics.Raycast(animator.GetIKPosition(AvatarIKGoal.LeftHand), transform.TransformDirection(Vector3.forward), out rayHit, 1 + offsetHand, layerMask);
            if (ray)
            {
                if (rayHit.collider)
                {
                    Vector3 handPosition = rayHit.point;
                    handPosition.z += offsetHand;
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, handPosition);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(transform.up, rayHit.normal));
                }
            }
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            ray = Physics.Raycast(animator.GetIKPosition(AvatarIKGoal.RightHand), transform.TransformDirection(Vector3.forward), out rayHit, 1 + offsetHand, layerMask);
            if (ray)
            {
                if (rayHit.collider)
                {
                    Vector3 handPosition = rayHit.point;
                    handPosition.z += offsetHand;
                    animator.SetIKPosition(AvatarIKGoal.RightHand, handPosition);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(transform.up, rayHit.normal));
                }
            }
        }
    }
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
    public void SetFootRay(Vector3 left, Vector3 right)
    {
        leftFoot = left;
        rightFoot = right;
    }
}
