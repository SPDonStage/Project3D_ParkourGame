using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ATMMachine : MonoBehaviour
{
    private Transform getItemDeposit;
    [SerializeField] private Animator animator;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private bool isDeposited;
    [SerializeField] private int timeValue;
    private void Awake()
    {
        animator ??= GetComponent<Animator>();
        videoPlayer ??= GetComponent<VideoPlayer>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isDeposited)
        {
            videoPlayer.enabled = true;
        }
        else
        {
            videoPlayer.enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isDeposited)
        {
            if (other.CompareTag("Player"))
            {
                getItemDeposit = other.gameObject.transform.Find("Item Holder");
                if (getItemDeposit != null)
                {
                    if (getItemDeposit.childCount != 0)
                    {
                        if (getItemDeposit.GetChild(0).CompareTag("Cash"))
                        {
                            animator.SetBool("isCash", true);
                            StartCoroutine(countDown(timeValue * 2));
                        }
                        if (getItemDeposit.GetChild(0).CompareTag("ATM Card"))
                        {
                            animator.SetBool("isCard", true);
                            StartCoroutine(countDown(timeValue));
                        }
                        
                        isDeposited = true;
                        Destroy(getItemDeposit.GetChild(0).gameObject);
                    }
                }
            }
        }
    }  
    IEnumerator countDown(int time)
    {
        while (time > 0)
        {
            Debug.Log(time);
            yield return new WaitForSeconds(1);
            time -= 1;
        }
        isDeposited = false;
    }
    public void setCashOff() //set in animation
    {
        animator.SetBool("isCash", false);
    }
    public void setCardOff() //set in animation
    {
        animator.SetBool("isCard", false);
    }
}
