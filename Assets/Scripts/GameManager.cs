using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //match time
    [SerializeField] private int matchTimeValue;
    private static int matchTime;
    int mininute;
    int second;
    //airdrop time
    [SerializeField] private GameObject airdropPrefab;
    private GameObject airdrop = null;
    [SerializeField] private int airdropTimeValue; //airdrop will be dropped in each airdropTimeValue time
    private static int airdropTime;
    [SerializeField]private bool isAirdropCD;
    private bool isStartCoroutine;
    IEnumerator airdropTimerCoroutine;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        matchTime = matchTimeValue;
        airdropTime = airdropTimeValue;
        isAirdropCD = true;
        isStartCoroutine = true;
        StartCoroutine(matchTimer());
        airdropTimerCoroutine = airdropTimer();
    }

    // Update is called once per frame
    void Update()
    {
        //match time
        if (matchTime > 0)
        {
            mininute = matchTime / 60;
            second = matchTime % 60;
        }
        //air drop
        if (airdropTime / 15 == 0)
        {
            if (!airdrop)
            {
                airdrop = Instantiate(airdropPrefab);
            }
        }
        if (!isAirdropCD)
        {
            StopCoroutine(airdropTimerCoroutine);
            isStartCoroutine = true;
        }
        else
        {
            if (isStartCoroutine)
            {
                StartCoroutine(airdropTimerCoroutine);
                isStartCoroutine = false;
            }
        }
    }
    IEnumerator matchTimer()
    {
        while (matchTime > 0)
        {
            yield return new WaitForSeconds(1);
            matchTime--;
        }
    }
    IEnumerator airdropTimer()
    {
        while (airdropTime > 0)
        {
            Debug.Log(airdropTime);
            yield return new WaitForSeconds(1);
            airdropTime--;
        }

    }
}
