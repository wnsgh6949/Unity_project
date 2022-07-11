using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrowTriggerable : MonoBehaviour {

    Transform _player;
    PlayerController _playerController;
    Animator _animator;
    LineRenderer _lineRenderer;

    public Rigidbody projectile;
    public Transform projectileSpawn;
    public float projectileForce = 1f;

    public LayerMask collisionMask;
    public Light directionalLight;
    public GameObject explosionRadius;

    void Start()
    {
        _player = transform.root;
        _playerController = _player.GetComponent<PlayerController>();
        _animator = _player.GetComponent<Animator>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void Aim()
    {
        _playerController.isThrowing = true;
        if(_playerController.isAiming)
        {
            StartCoroutine(_playerController.ModeSwitch());
        }
        _playerController.StopPlayer(true);
        StartCoroutine(Launch());
    }

    IEnumerator Launch()
    {
        Time.timeScale = 0.2f;
        directionalLight.color = new Color(0.5f, 0.5f, 0.5f, 1);
        _lineRenderer.enabled = true;
        GameObject erInstance = Instantiate(explosionRadius, transform.position, Quaternion.identity) as GameObject;
        while(Input.GetButton("Q"))
        {
            _animator.SetBool("IsLaunching", true);
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit target, 100, collisionMask))
            {
                Vector3 lookAtPoint = new Vector3(target.point.x, _player.position.y, target.point.z);
                float yForAtan = 0.2f;
                float xForAtan = Vector3.Magnitude(lookAtPoint - _player.position);
                float alpha = Mathf.Atan2(yForAtan,xForAtan) * Mathf.Rad2Deg;
                _player.LookAt(lookAtPoint);
                _player.Rotate(0, -alpha, 0);

                Vector3 drawPoint = projectileSpawn.position;
                float timeToTarget = Mathf.Sqrt(-2*projectileSpawn.position.y/Physics.gravity.y);
                int resolution = 30;
                _lineRenderer.SetPosition(0, projectileSpawn.position);
                for(int i=1; i<=resolution; i++)
                {
                    float simulationTime = i / (float)resolution * timeToTarget;
                    Vector3 displacement = transform.forward * projectileForce * simulationTime + Physics.gravity * simulationTime * simulationTime / 2f;
                    drawPoint = projectileSpawn.position + displacement;
                    _lineRenderer.SetPosition(i, drawPoint);
                    if(Physics.OverlapSphere(drawPoint, 0.05f).Length > 0)
                    {
                        if(Physics.OverlapSphere(drawPoint, 0.05f)[0].name != "ShootingPlane")
                        {
                            _lineRenderer.positionCount = i+1;
                            break;
                        }
                    }
                }
                erInstance.transform.position = drawPoint;
            }
            yield return null;
        }
        Destroy(erInstance, 0.5f);
        _lineRenderer.enabled = false;
        directionalLight.color = new Color(1, 1, 1, 1);
        Time.timeScale = 1;

        yield return new WaitForSeconds(0.5f);

        Rigidbody newProjectile = Instantiate(projectile, projectileSpawn.position, transform.rotation) as Rigidbody;
        newProjectile.velocity = transform.forward * projectileForce;

        _animator.SetBool("IsLaunching", false);
        _playerController.isThrowing = false;
        _playerController.StopPlayer(false);
        yield return new WaitForSeconds(0.5f);
    }
}