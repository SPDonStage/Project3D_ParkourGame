using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform gunBarrel;
    public bool canShoot = true;
    public int Ammo;
    public int DefaultAmmo;
    public TextMeshProUGUI ReloadText;
    public float reloadTime = 3.5f;
    public bool isReloading = false;
    public TextMeshProUGUI TextAmmo;
    public float bulletSpeed = 1000f;

    private Camera mainCamera;

    private void Start()
    {
        Ammo = DefaultAmmo;
        mainCamera = Camera.main; // Cache the main camera
    }

    private void Update()
    {
        if (canShoot)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(1000); // A far point in the direction of the crosshair
            }

            Vector3 direction = (targetPoint - gunBarrel.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, Quaternion.LookRotation(direction));
            bullet.GetComponent<Rigidbody>().AddForce(direction * bulletSpeed, ForceMode.Force);
            Destroy(bullet, 5);
            AmmoManager();
            StartCoroutine(ShootDelayTime());
        }
    }

    public void AmmoManager()
    {
        Ammo--;
        TextAmmo.SetText(Ammo.ToString());

        if (Ammo == 0 || Input.GetKey(KeyCode.R))
        {
            Reload();
            StartCoroutine(ReloadTime());
        }
    }

    void Reload()
    {
        Ammo = DefaultAmmo;
    }

    IEnumerator ReloadTime()
    {
        isReloading = true;
        ReloadText.text = "Reloading...";
        canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        ReloadText.text = "Reload complete!";
        isReloading = false;
        TextAmmo.SetText(Ammo.ToString());
        canShoot = true;
    }

    IEnumerator ShootDelayTime()
    {
        canShoot = false;
        yield return new WaitForSeconds(1);
        canShoot = true;
    }
}
