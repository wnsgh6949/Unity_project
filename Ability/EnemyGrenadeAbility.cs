using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrenadeAbility : MonoBehaviour {

    Transform _target;
    Transform _enemy;
    EnemyController _enemyController;
    Animator _animator;
    LineRenderer _lineRenderer;

    public Grenade projectile;
    public Transform projectileSpawn;
    public LayerMask collisionMask;
    public GameObject explosionRadius;
    public GameObject gun;

    float coolDownDuration = 5f;
    float coolDownTimeLeft;
    float h = 1f;
    bool hasTarget = false;

    struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
    }

    void Start()
    {
        _enemy = transform.root;
        _enemyController = _enemy.GetComponent<EnemyController>();
        _animator = _enemy.GetComponent<Animator>();
        _lineRenderer = GetComponent<LineRenderer>();

        if((_target = GameObject.FindWithTag("Player").transform) != null)
        {
            hasTarget = true;
            _target.GetComponent<HPController>().OnDeath += OnTargetDeath;
        }
        coolDownTimeLeft = coolDownDuration;
    }

    void Update () 
    {
        coolDownTimeLeft -= Time.deltaTime;
        if (coolDownTimeLeft <= 0 && _enemyController.isAiming && hasTarget) 
        {
            Aim();
            coolDownTimeLeft = 100f;
        }
    }

    void OnTargetDeath()
    {
        if(hasTarget)
        {
            hasTarget = false;
            this.enabled = false;
        }
    }

    void Aim()
    {
        StartCoroutine(_enemyController.ModeSwitch());
        _enemyController.enabled = false;
        StartCoroutine(DrawPath());
    }

    IEnumerator DrawPath()
    {
        _lineRenderer.enabled = true;
        LaunchData launchData = CalculateLaunchData(_target.position);
        GameObject erInstance = Instantiate(explosionRadius, transform.position, Quaternion.identity) as GameObject;
        Destroy(erInstance, 1.5f + launchData.timeToTarget);

        int resolution = 30;
        _lineRenderer.SetPosition(0, projectileSpawn.position);
        for(int i=1; i<=resolution; i++)
        {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 displacement = launchData.initialVelocity * simulationTime + Physics.gravity * simulationTime * simulationTime /2f;
            Vector3 drawPoint = projectileSpawn.position + displacement;
            _lineRenderer.SetPosition(i, drawPoint);
        }
        erInstance.transform.position = projectileSpawn.position + launchData.initialVelocity * launchData.timeToTarget + Physics.gravity * launchData.timeToTarget * launchData.timeToTarget /2f + new Vector3(0, 0.01f, 0);
        yield return new WaitForSeconds(1.5f);
        _lineRenderer.enabled = false;
        StartCoroutine(Launch(launchData.initialVelocity));
    }

    IEnumerator Launch(Vector3 initialVelocity)
    {
        gun.SetActive(false);
        _animator.SetTrigger("IsThrowing");
        yield return new WaitForSeconds(0.5f);

        Grenade newProjectile = Instantiate(projectile, projectileSpawn.position, Quaternion.identity) as Grenade;
        newProjectile.velocity = initialVelocity;
        newProjectile.thrower = transform.root;
        _enemyController.enabled = true;
        StartCoroutine(_enemyController.ModeSwitch());
        coolDownTimeLeft = coolDownDuration;
        yield return new WaitForSeconds(0.5f);
        gun.SetActive(true);
    }

    LaunchData CalculateLaunchData(Vector3 _target)
    {
        float displacementY = _target.y - projectileSpawn.position.y;
        Vector3 displacementXZ = new Vector3(_target.x - projectileSpawn.position.x, 0, _target.z - projectileSpawn.position.z);
        float time = Mathf.Sqrt(-2*h/Physics.gravity.y) + Mathf.Sqrt(2*(displacementY - h)/Physics.gravity.y);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * h);
        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(Physics.gravity.y), time);
    }
}