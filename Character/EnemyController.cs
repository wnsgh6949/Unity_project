using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    enum State
    {
        IDLE,
        PURSING,
        ATTACKING
    }

    Animator _animator;
    NavMeshAgent _agent;
    EnemyShootingController _shootingController;
    HPController _hpController;
    Transform _target;
    HPController _targetHP;

    public GameObject uiPrefab;

    // Start()
    Vector3 _startingAnchor;
    bool hasTarget = false;

    // Update()
    public bool isAiming = false;
    bool isSwitching = false;
    bool promisingCover = false;
    float distToPlayer;
    //float pursuitTimer = 0f;
    float detectionRadius = 7.5f;
    float weaponRange = 5f;
    State currentState;
    Coroutine updatePath;
    Text ui;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _shootingController = GetComponent<EnemyShootingController>();
        _hpController = GetComponent<HPController>();

        _shootingController.enabled = false;
        _hpController.OnDamaged += OnDamaged;
        _startingAnchor = transform.position;

        if((_target = GameObject.FindWithTag("Player").transform) != null)
        {
            hasTarget = true;
            _target.GetComponent<HPController>().OnDeath += OnTargetDeath;
        }
    }
    
    void Update()
    {
        if(isSwitching)
        {
            return;
        }

        if(hasTarget)
        {
            distToPlayer = Vector3.SqrMagnitude(_target.position - transform.position);

            switch(currentState)
            {
                case State.IDLE:
                {
                    if(distToPlayer < detectionRadius * detectionRadius) // player is in dection radius
                    {
                        currentState = State.PURSING;
                        StopCoroutine(updatePath);
                        updatePath = StartCoroutine(UpdatePath(true));

                        foreach(Canvas c in FindObjectsOfType<Canvas>())
                        {
                            if(c.renderMode == RenderMode.WorldSpace)
                            {
                                ui = Instantiate(uiPrefab, c.transform).GetComponent<Text>();
                                ui.text = "!";
                                ui.color = Color.red;
                                ui.transform.position = transform.position + Vector3.up + Vector3.right;
                                break;
                            }
                        }
                    }
                    else
                    {
                        updatePath = StartCoroutine(UpdatePath(false));
                    }
                }
                break;
                case State.PURSING:
                {
                    /*if(distToPlayer < detectionRadius * detectionRadius) // player still in detection radius
                    {
                        pursuitTimer = 4f;
                    }
                    else // player is not in detection radius
                    {
                        if(pursuitTimer > 0.0f)
                        {
                            pursuitTimer -= Time.deltaTime;

                            if(pursuitTimer <= 0f)
                            {
                                currentState = State.IDLE;
                                StopCoroutine(updatePath);
                            }
                        }
                    }*/
                }
                break;
                case State.ATTACKING:
                {
                    if(weaponRange * weaponRange < distToPlayer) // player is not in weapon range
                    {
                        currentState = State.PURSING;
                        StartCoroutine(ModeSwitch());
                        updatePath = StartCoroutine(UpdatePath(true));
                    }
                    LookAtTarget();
                }
                break;
            }
        }

        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }

    public IEnumerator ModeSwitch()
    {
        float switchDuration = 0.6f;
        
        LookAtTarget();
        isSwitching = true;
        StopEnemy(true);
        isAiming = !isAiming;
        _animator.SetBool("IsAiming", isAiming);

        if(!_shootingController.enabled)
        {
            weaponRange++;
            yield return new WaitForSeconds(switchDuration);
        }
        else
        {
            weaponRange--;
        }

        _shootingController.enabled = !_shootingController.enabled;
        StopEnemy(false);
        isSwitching = false;
    }

    void OnDamaged()
    {
        if(currentState == State.IDLE)
        {
            //pursuitTimer = 4f;
            currentState = State.PURSING;
            StopCoroutine(updatePath);
            updatePath = StartCoroutine(UpdatePath(true));
        }
    }

    void OnTargetDeath()
    {
        if(hasTarget)
        {
            hasTarget = false;
            Destroy(_shootingController);
            this.enabled = false;
        }
    }

    IEnumerator UpdatePath(bool pursing)
    {
        float refreshRate = 0.25f;

        if(pursing)
        {
            if(Random.value < 0.5f)
            {
                StartCoroutine(SearchCover());
                yield break;
            }
            while(_target != null)
            {
                if(distToPlayer < weaponRange * weaponRange) // player is now in weapon range
                {
                    currentState = State.ATTACKING;
                    StartCoroutine(ModeSwitch());
                    break;
                }
                
                transform.LookAt(_target.position);
                _agent.SetDestination(_target.position);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            _agent.SetDestination(transform.position + Vector3.left);
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator SearchCover()
    {
        float refreshRate = 0.25f;

        while(true)
        {
            if(promisingCover)
            {
                if(transform.position == _agent.destination)
                {
                    promisingCover = false;
                    currentState = State.ATTACKING;
                    StartCoroutine(ModeSwitch());
                    break;
                }
            }
            else
            {
                Collider[] colliders_1 = Physics.OverlapSphere(_target.position, weaponRange - 0.5f);
                Collider[] colliders_2 = Physics.OverlapSphere(transform.position, 5);
                foreach(Collider nearbyObject_1 in colliders_1)
                {
                    if(nearbyObject_1.tag == "Cover")
                    {
                        foreach(Collider nearbyObject_2 in colliders_2)
                        {
                            if(nearbyObject_1 == nearbyObject_2 && !nearbyObject_1.transform.GetChild(1).GetComponent<CoverZone>().occupied)
                            {
                                promisingCover = true;
                                nearbyObject_1.transform.GetChild(1).GetComponent<CoverZone>().occupied = true;
                                _agent.SetDestination(nearbyObject_1.transform.GetChild(1).position - new Vector3(0, 0.375f, 0));
                                break;
                            }
                        }
                    }
                    if(promisingCover)
                    {
                        break;
                    }
                }
                if(!promisingCover)
                {
                    updatePath = StartCoroutine(UpdatePath(true));
                    break;
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    void LookAtTarget()
    {
        float yForAtan = 0.1f;
        float xForAtan = Vector3.Magnitude(_target.position - transform.position);
        float alpha = Mathf.Atan2(yForAtan,xForAtan) * Mathf.Rad2Deg;
        transform.LookAt(_target.position);
        transform.Rotate(new Vector3(0, -alpha, 0));
    }

    public void StopEnemy(bool stop)
    {
        if(stop)
        {
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
            _agent.isStopped = true;
            _animator.SetFloat("Speed", 0);
        }
        else
        {
            _agent.isStopped = false;
        }
    }
}
