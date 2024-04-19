using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

[RequireComponent(typeof(PlayerController))]
[RequireComponent (typeof(CharacterController))]
public class EnvironmentChecker : MonoBehaviour
{
    private CharacterController characterController;
    private Vector3 Yoffset_Ray = Vector3.zero;
    private float Yoffset_Ray_Length = 2f;
    [SerializeField] private LayerMask layerMask;
    private Vector3 originPos;
    private Vector3 originPosDownward;
    private ObjectData objectData;
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<PlayerController>().enabled = true;
        characterController ??= GetComponent<CharacterController>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public ObjectData checkData()
    {
        objectData = new ObjectData();
        originPos = transform.position;
        objectData.Yoffset_Ray_Hit_Check = Physics.Raycast(originPos, transform.forward, out objectData.Yoffset_Ray_Hit,
                        Yoffset_Ray_Length, layerMask);
        if (objectData.Yoffset_Ray_Hit.collider)
        {
            originPosDownward = objectData.Yoffset_Ray_Hit.point + (transform.forward * 0.05f) + new Vector3(0, characterController.height + .5f, 0);
            objectData.Downward_Ray_Hit_Check = Physics.Raycast(originPosDownward, Vector3.down, out objectData.Downward_Ray_Hit, characterController.height + .5f, layerMask);
            objectData.rotationToRotate = objectData.Yoffset_Ray_Hit.point - originPos;
        }    
        return objectData;
    }
    public void OnDrawGizmos()
    {
        objectData = new ObjectData();
        originPos = transform.position;
        objectData.Yoffset_Ray_Hit_Check = Physics.Raycast(originPos, transform.forward, out objectData.Yoffset_Ray_Hit,
                        Yoffset_Ray_Length, layerMask);
        if (objectData.Yoffset_Ray_Hit.collider)
        {
            originPosDownward = objectData.Yoffset_Ray_Hit.point + (transform.forward * 0.05f) + new Vector3(0, characterController.height + .5f, 0);
            objectData.Downward_Ray_Hit_Check = Physics.Raycast(originPosDownward, Vector3.down, out objectData.Downward_Ray_Hit, characterController.height + .5f, layerMask);
            Gizmos.color = Color.blue; 
            Gizmos.DrawRay(transform.position, objectData.Yoffset_Ray_Hit.point);
            if (objectData.Downward_Ray_Hit.collider)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(originPosDownward, objectData.Downward_Ray_Hit.point);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(originPosDownward, -transform.up);
            }
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(originPos, transform.forward);
        }
        
    }
    public struct ObjectData
    {
        public bool Yoffset_Ray_Hit_Check;
        public bool Downward_Ray_Hit_Check;
        public RaycastHit Yoffset_Ray_Hit;
        public RaycastHit Downward_Ray_Hit;
        public Vector3 rotationToRotate;
    }
}
