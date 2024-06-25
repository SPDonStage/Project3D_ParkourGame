using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class SkillManagement : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private LayerMask layerMask;
    // Start is called before the first frame update
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Walling();
        Turret();
        ShieldOn();
    }
    private void FixedUpdate()
    {
 
    }
    [Header("-=-Dash-=-")]
    [SerializeField] private float dashForce;
    public IEnumerator Dash()
    {
        float timer = 0;
        while (timer <= 1)
        {
            playerController.characterController.Move(transform.TransformDirection((Vector3.forward * dashForce) * Time.deltaTime));
            timer += Time.fixedDeltaTime;
            yield return null;
        }
    }
    //HIGH JUMP
    [Header("-=-HIGH JUMP-=-")]
    [SerializeField] private int highJumpCD_Value;
    public void HighJump()
    {
        GetComponent<PlayerController>().canJump = true;
        GetComponent<PlayerController>().onJumping = true;
        GetComponent<PlayerController>().jumpForce *= 1.5f;
    }
    public IEnumerator HighJumpCD()
    {
        int highJumpCD = highJumpCD_Value;
        if (highJumpCD != 0)
        {
            
        }
        else
        {
            highJumpCD = highJumpCD_Value;
        }
        
        while (highJumpCD != 0)
        {
            highJumpCD--;
            yield return new WaitForSeconds(1);
        }    
    }
    //WALLING
    [Header("-=-WALLING-=-")]
    [SerializeField] private int wallingCD_Value;
    [SerializeField] private GameObject wallReview_Prefab;
    [SerializeField] private GameObject wall_Prefab;
    GameObject wallReview;
    SageWall wallSummon;
    public bool isWallReview = false;
    public void Walling()
    {
        if (isWallReview)
        {
            if (!wallReview)
            {
                wallReview = Instantiate(wallReview_Prefab);
            }
            else
            {
                wallReview.transform.position = transform.position + transform.forward * 3;
                wallReview.transform.rotation = transform.rotation;
                Physics.Raycast(wallReview.transform.position, wallReview.transform.forward, out RaycastHit wallReviewRayCast, 5);
                if (wallReview.gameObject.activeSelf)
                {
                    if (playerController.playerInput.Player.MouseClick.triggered)
                    {
                        wallSummon = Instantiate(wall_Prefab, wallReview.transform.position, wallReview.transform.rotation).GetComponent<SageWall>();
                        if (wallReviewRayCast.collider)
                        {
                            wallSummon.transformInIt.z = Vector3.Distance(wallReview.transform.position, wallReviewRayCast.point); //wall's size is cut when collided
                        }
                        else
                        {
                            wallSummon.transformInIt.z = 5;
                        }
                        
                        Destroy(wallReview.gameObject);
                        StartCoroutine(WallingCD());
                        isWallReview = false;
                    }
                }             
            }
        }
    }
    public IEnumerator WallingCD() 
    {
        int wallingCD = wallingCD_Value;
        
        if (wallingCD != 0)
        {
      
        }
        else
        {
            wallingCD = wallingCD_Value;
        }
        while (wallingCD != 0)
        {
            wallingCD--;
            yield return new WaitForSeconds(1);
        }
    }
    //THROW SMOKE
    [Header("-=-THROW SMOKE-=-")]
    [SerializeField] private GameObject smokePrefab;
    GameObject smoke;
    public void ThrowSmoke()
    {
        if (!smoke)
        {           
            smoke = Instantiate(smokePrefab, playerController.camera.transform.position + transform.forward, Quaternion.identity);
            smoke.GetComponent<ThrowSmoke>().playerController = this.playerController;
        }
        else
        {

        }
      

    }
    //TURRET
    [Header("-=-TURRET-=-")]
    [SerializeField] private GameObject turretReview_Prefab;
    [SerializeField] private GameObject turret_Prefab;
    GameObject turretReview;
    Turret turretSummon;
    public bool isTurretReview = false;
    public void Turret()
    {
        if (isTurretReview)
        {
            if (!turretReview)
            {
                turretReview = Instantiate(turretReview_Prefab);
            }
            else
            {
            //    Physics.Raycast(transform.position, transform.forward, out RaycastHit checkForward, 3, layerMask);
                turretReview.transform.position = transform.position + transform.forward * 3;
                turretReview.transform.rotation = transform.rotation;
                if (turretReview.gameObject.activeSelf && !turretReview.GetComponent<ObjectReview>().isCollided)
                {
                    if (playerController.playerInput.Player.MouseClick.triggered)
                    {
                        turretSummon = Instantiate(turret_Prefab, turretReview.transform.position, turretReview.transform.rotation).GetComponent<Turret>();
                        Destroy(turretReview.gameObject);
                        isTurretReview = false;
                    }
                }
            }
        }
       
    }
    //SHIELD
    [Header("-=-SHIELD-=-")]
    [SerializeField] private GameObject shield_Prefab;
    private GameObject shieldGameObject;
    public bool shieldOn;
    public void ShieldOn()
    {
        if (shieldOn)
        {
           
            if (!shieldGameObject)
                shieldGameObject = Instantiate(shield_Prefab, transform.position + transform.forward * 3, transform.rotation);
            else
            {
                shieldGameObject.GetComponent<Shield>().isRestored = false;
                shieldGameObject.transform.GetChild(0).gameObject.SetActive(true); //shield object is child
                shieldGameObject.transform.parent = transform;
                shieldGameObject.transform.position = transform.position + transform.forward * 3; 
                shieldGameObject.transform.rotation = transform.rotation;
            }
        }
    }
    public void ShieldOff()
    {
        if (!shieldOn)
        {
            if (shieldGameObject)
            {
                shieldGameObject.GetComponent<Shield>().isRestored = true;
                shieldGameObject.GetComponent<Shield>().isRestoring = true;
                shieldGameObject.GetComponent<Shield>().shieldGameObject.SetActive(false);
            }
        }
    }
}
