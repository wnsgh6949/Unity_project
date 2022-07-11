using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class ShootingController : MonoBehaviour
{
    PlayerController _playerController;
    LineRenderer _laserLine;
    Animator _animator;
    UIAmmoBar _uiAmmoBar;
    Muzzleflash _muzzleFlash;
    CameraMovement _cameraMovement;

    public Transform gunEnd;
    public Transform shell;
    public Transform shellEjection;
    public Projectile projectile;
    public Text ammoUI;
    public float fireRate = 0.2f;
    public int bulletDmg = 20;
    public int maxAmmo = 20;
    public int currentAmmo;
    
    // Update()
    bool colorChange = false;
    float nextFire;

    // Shoot()
    public bool rapidFireMode = false;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _laserLine = GetComponent<LineRenderer>();
        _animator = GetComponent<Animator>();
        _uiAmmoBar = GetComponent<UIAmmoBar>();
        _muzzleFlash = GetComponent<Muzzleflash>();
        _cameraMovement = FindObjectOfType<CinemachineVirtualCamera>().GetComponent<CameraMovement>();

        ResetAmmo();
    }

    void Update()
    {
        if(!_playerController.enabled || _playerController.isReloading)
        {
            return;
        }

        LaserSight();
        
        if(Input.GetButton("Fire1"))
        {
            if(currentAmmo == 0)
            {
                StartCoroutine(_playerController.Reload());
                return;
            }

            if(Time.time > nextFire)
            {
                Shoot();
            }
        }

        if(Input.GetButtonDown("Fire3")) // change laser color
        {
            if(colorChange)
            {
                _laserLine.startColor = Color.red;
                _laserLine.endColor = Color.red;
            }
            else
            {
                _laserLine.startColor = Color.green;
                _laserLine.endColor = Color.green;
            }
            colorChange = !colorChange;
        }
    }

    void LaserSight()
    {
        _laserLine.SetPosition(0, gunEnd.position);
        if(Physics.Raycast(gunEnd.position, transform.forward, out RaycastHit hit, 50))
        {
            _laserLine.SetPosition(1, hit.point);
        }
        else
        {
            _laserLine.SetPosition(1, transform.forward * 10 + gunEnd.position);
        }
    }

    void Shoot()
    {
        nextFire = Time.time + fireRate;
        Projectile newProjectile = Instantiate(projectile, gunEnd.position, transform.rotation) as Projectile;
        newProjectile.SetDamage(bulletDmg);
        newProjectile.fromPlayer = true;
        Instantiate(shell, shellEjection.position, transform.rotation);

        StartCoroutine(_cameraMovement.CameraShake(0.2f));
        AudioManager.instance.PlaySound("Shoot", transform.position);
        _muzzleFlash.Activate();
        if(!rapidFireMode)
        {
            currentAmmo--;
        }

        SetAmmoUI();
    }

    public void SetAmmoUI()
    {
        _uiAmmoBar.SetValue(currentAmmo / (float)maxAmmo);
        ammoUI.text = currentAmmo + " / " + (float)maxAmmo;
    }

    public void ResetAmmo()
    {
        currentAmmo = maxAmmo;
        SetAmmoUI();
    }
}