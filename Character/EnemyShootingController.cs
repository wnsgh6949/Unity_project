using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootingController : MonoBehaviour
{
    Animator _animator;
    Muzzleflash _muzzleFlash;
    Transform _target;

    public Transform gunEnd;
    public Projectile projectile;
    public float fireRate = 0.4f;
    public int maxAmmo = 20;
    int currentAmmo;

    // Update()
    float nextFire;
    bool isReloading = false;


    void Start()
    {
        _animator = GetComponent<Animator>();
        _muzzleFlash = GetComponent<Muzzleflash>();

        currentAmmo = maxAmmo;
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if(isReloading)
        {
            return;
        }

        if(currentAmmo == 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(Time.time > nextFire)
        {
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        float reloadDuration = 1f;

        isReloading = true;
        _animator.SetTrigger("IsReloading");
        yield return new WaitForSeconds(reloadDuration);
        AudioManager.instance.PlaySound("Reload", transform.position);
        yield return new WaitForSeconds(reloadDuration);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        nextFire = Time.time + fireRate;
        Projectile newProjectile = Instantiate(projectile, gunEnd.position, transform.rotation) as Projectile;
        _muzzleFlash.Activate();
        AudioManager.instance.PlaySound("Shoot", transform.position);
        currentAmmo--;
    }
}
