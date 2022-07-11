using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent _agent;
    Animator _animator;
    ShootingController _shootingController;
    UIAmmoBar _uiAmmoBar;
    LineRenderer _laserLine;

    public GameObject crosshair;

    // Update()
    bool isSwitching = false;
    public bool isReloading = false;
    public bool isAiming = false;
    public bool isThrowing = false;
    public GameObject arrow;
    float speed;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _shootingController = GetComponent<ShootingController>();
        _uiAmmoBar = GetComponent<UIAmmoBar>();
        _laserLine = GetComponent<LineRenderer>();

        _shootingController.enabled = false;
    }

    void Update()
    {
        if(isAiming)
        {
            if(!Input.GetMouseButton(1)) // Character is aiming when it is not intended
            {
                StartCoroutine(ModeSwitch());
                return;
            }
            LookAtTarget();
        }

        if(isSwitching)
        {
            return;
        }

        if(Input.GetMouseButtonDown(1)) 
        {
            if(!isThrowing && !isReloading) // Can't shoot while throwing/reloading
            {
                StartCoroutine(ModeSwitch());
                return;
            }
        }

        if(Input.GetMouseButton(0) && !_shootingController.enabled) // Can move while not aiming
        {
            Move();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if(!isReloading && _shootingController.currentAmmo != _shootingController.maxAmmo)
            {
                StartCoroutine(Reload());
            }
        }

        speed = _agent.velocity.magnitude;
        _animator.SetFloat("Speed", speed);
    }

    public IEnumerator ModeSwitch()
    {
        float switchDuration = 0.5f;

        isSwitching = true;
        StopPlayer(true);
        crosshair.SetActive(!isAiming);
        isAiming = !isAiming;
        _animator.SetBool("IsAiming", isAiming);

        if(!_shootingController.enabled)
        {
            yield return new WaitForSeconds(switchDuration);
        }

        _laserLine.enabled = isAiming;
        _shootingController.enabled = !_shootingController.enabled;
        StartCoroutine(_uiAmmoBar.FadeUI(!isAiming));
        StopPlayer(false);
        isSwitching = false;
    }

    void Move()
    {
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit1, 100, LayerMask.GetMask("CoverZone")))
        {
            _agent.destination = hit1.transform.position;
            arrow.SetActive(true);
            arrow.transform.position = hit1.transform.position;
        }
        else if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit2, 100, LayerMask.GetMask("Floor")))
        {
            _agent.destination = hit2.point;
            arrow.SetActive(true);
            arrow.transform.position = hit2.point;
        }
    }

    public IEnumerator Reload()
    {
        float reloadDuration = 1f;
        
        _laserLine.enabled = false;
        isReloading = true;
        _animator.SetTrigger("IsReloading");
        yield return new WaitForSeconds(reloadDuration);
        AudioManager.instance.PlaySound("Reload", transform.position);
        yield return new WaitForSeconds(reloadDuration);

        _shootingController.ResetAmmo();
        _shootingController.SetAmmoUI();
        isReloading = false;
        if(_shootingController.enabled)
        {
            _laserLine.enabled = true;
        }
    }

    void LookAtTarget()
    {
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, LayerMask.GetMask("ShootingPlane")))
        {
            Vector3 lookAtPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            float yForAtan = 0.2f;
            float xForAtan = Vector3.Magnitude(lookAtPoint - transform.position);
            float alpha = Mathf.Atan2(yForAtan,xForAtan) * Mathf.Rad2Deg;
            transform.LookAt(lookAtPoint);
            transform.Rotate(0, -alpha, 0);
        }
    }

    public void StopPlayer(bool stop)
    {
        if(stop)
        {
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
            _agent.isStopped = true;
            arrow.SetActive(false);
        }
        else
        {
            _agent.isStopped = false;
        }
    }
}