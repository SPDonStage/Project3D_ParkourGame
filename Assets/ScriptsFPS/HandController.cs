using NUnit.Framework.Internal.Builders;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour
{
    [Header("CrossHair")]
    public Image CrossHair;
    public Image CrossHairSnip;
    public GameObject NormalCV;
    public GameObject SniperCV;

    [Header("Gun")]
    public GameObject RiflePrefab;
    public GameObject ShotGunPrefab;
    public GameObject SMGPrefab;
    public GameObject SniperPrefab;

    [Header("Camera")]
    public GameObject NormalCam;
    public GameObject SniperCam;

    private WeaponInfo currentWeaponInfo;

    private GameObject CurrentGun;

    private void Start()
    {
        // Initialize the current gun as Sniper by default
        CurrentGun = SniperPrefab;
        currentWeaponInfo = CurrentGun.GetComponent<WeaponInfo>();
    }

    void Update()
    {
        if (currentWeaponInfo != null)
        {
            currentWeaponInfo.Shoot();
            if (CurrentGun == SniperPrefab)
            {
                ZoomIn();
                ZoomOut();
            }
        }
        SwapGun();
    }

    void ZoomIn()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            NormalCV.SetActive(false);
            SniperCV.SetActive(true);
            NormalCam.SetActive(false);
            SniperCam.SetActive(true);
            currentWeaponInfo.Shoot();
        }
    }

    void ZoomOut()
    {
        if (Input.GetButtonUp("Fire2"))
        {
            NormalCV.SetActive(true);
            SniperCV.SetActive(false);
            NormalCam.SetActive(true);
            SniperCam.SetActive(false);
        }
    }

    void SwapGun()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentGun = RiflePrefab;
            currentWeaponInfo = RiflePrefab.GetComponent<WeaponInfo>();
            RiflePrefab.SetActive(true);
            ShotGunPrefab.SetActive(false);
            SMGPrefab.SetActive(false);
            SniperPrefab.SetActive(false);
            CrossHair.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentGun = ShotGunPrefab;
            currentWeaponInfo = ShotGunPrefab.GetComponent<WeaponInfo>();
            RiflePrefab.SetActive(false);
            ShotGunPrefab.SetActive(true);
            SMGPrefab.SetActive(false);
            SniperPrefab.SetActive(false);
            CrossHair.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentGun = SMGPrefab;
            currentWeaponInfo = SMGPrefab.GetComponent<WeaponInfo>();
            RiflePrefab.SetActive(false);
            ShotGunPrefab.SetActive(false);
            SMGPrefab.SetActive(true);
            SniperPrefab.SetActive(false);
            CrossHair.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CurrentGun = SniperPrefab;
            currentWeaponInfo = SniperPrefab.GetComponent<WeaponInfo>();
            RiflePrefab.SetActive(false);
            ShotGunPrefab.SetActive(false);
            SMGPrefab.SetActive(false);
            SniperPrefab.SetActive(true);
            CrossHair.enabled = false;
        }
    }
}
