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
    private Vector3 Yoffset_Ray = new Vector3(0, 0.1f, 0);
    private float Yoffset_Ray_Length = 2f;
    [SerializeField] private LayerMask layerMask;
    private Vector3 originPos;
    private Vector3 originPosDownward;
    private ObjectData objectData;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayerController>().enabled = true;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public ObjectData checkData()
    {
        objectData = new ObjectData();
        originPos = transform.position + Yoffset_Ray;
        objectData.Yoffset_Ray_Hit_Check = Physics.Raycast(originPos, transform.forward, out objectData.Yoffset_Ray_Hit,
                        Yoffset_Ray_Length, layerMask);
        if (objectData.Yoffset_Ray_Hit.collider != null)
        {
            originPosDownward = objectData.Yoffset_Ray_Hit.point + new Vector3(0, characterController.height + .5f, 0);
            objectData.Downward_Ray_Hit_Check = Physics.Raycast(originPosDownward, Vector3.down, out objectData.Downward_Ray_Hit, characterController.height + .5f, layerMask);

            Debug.DrawRay(originPosDownward, -transform.up * (characterController.height + .5f), objectData.Downward_Ray_Hit.collider != null ? Color.red : Color.green);
        }
        Debug.DrawRay(originPos, transform.forward * Yoffset_Ray_Length, objectData.Yoffset_Ray_Hit.collider != null ? Color.red : Color.green);
        Debug.DrawRay(originPos, objectData.Yoffset_Ray_Hit.point, Color.yellow);
        return objectData;
    }
    public struct ObjectData
    {
        public bool Yoffset_Ray_Hit_Check;
        public bool Downward_Ray_Hit_Check;
        public RaycastHit Yoffset_Ray_Hit;
        public RaycastHit Downward_Ray_Hit;
    }
}
