using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    Animator animator;
    bool ray;
    RaycastHit rayHit;
    [SerializeField] float offsetHit;[SerializeField] float offsetHit2;
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
            ray = Physics.Raycast(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down, out rayHit,1 + offsetHit, layerMask);       
            if (ray)
            {
                if (rayHit.collider)
                {
                    Vector3 footPosition = rayHit.point;
                    footPosition.y += offsetHit;
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, rayHit.normal));
                }
            }           
            //Right Foot
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            ray = Physics.Raycast(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down, out rayHit,1 + offsetHit, layerMask);         
            if (ray)
            {
                if (rayHit.collider)
                {
                    Vector3 footPosition = rayHit.point;
                    footPosition.y += offsetHit;
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rayHit.normal));
                }
            }
            if (isWallRun)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                ray = Physics.Raycast(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + transform.TransformDirection(leftFoot), transform.TransformDirection(rightFoot), out rayHit, 1 + offsetHit, layerMask); 
                Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + transform.TransformDirection(leftFoot), transform.TransformDirection(rightFoot),Color.green,2);
                if (ray)
                {
                    if (rayHit.collider)
                    {
                        Vector3 footPosition = rayHit.point;
                        footPosition.x += offsetHit2;
                        animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                        animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, rayHit.normal));
                    }
                }

                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                ray = Physics.Raycast(animator.GetIKPosition(AvatarIKGoal.RightFoot) + transform.TransformDirection(leftFoot), transform.TransformDirection(rightFoot), out rayHit, 1 + offsetHit, layerMask); 
                Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.RightFoot) + transform.TransformDirection(leftFoot), transform.TransformDirection(rightFoot), Color.red, 2);
                if (ray)
                {
                    if (rayHit.collider)
                    {
                        Vector3 footPosition = rayHit.point;
                        footPosition.y += offsetHit2;
                        animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                        animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rayHit.normal));
                    }
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
