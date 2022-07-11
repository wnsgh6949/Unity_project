using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverZone : MonoBehaviour
{
    public bool side;
    public bool occupied = false;
    float nextHeal;
    float nextRegen;
    float healCooldown = 5;
    float regenCooldown = 0.5f;
    Coroutine regenerate;

    void OnTriggerEnter(Collider other)
    {
        HPController _controller = other.GetComponent<HPController>();
        if(_controller != null)
        {
            if(side && _controller.gameObject.tag == "Player")
            {
                _controller.isCovered = true;
                other.GetComponent<Animator>().SetBool("IsCovered", true);
                if(Time.time > nextHeal)
                {
                    _controller.Heal();
                    nextHeal = Time.time + healCooldown;
                }
            }
            else if(!side && _controller.gameObject.tag == "Enemy")
            {
                _controller.isCovered = true;
                other.GetComponent<Animator>().SetBool("IsCovered", true);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        HPController _controller = other.GetComponent<HPController>();
        if(_controller != null && Time.time > nextRegen)
        {
            _controller.Regenerate();
            nextRegen = Time.time + regenCooldown;
        }
    }

    void OnTriggerExit(Collider other)
    {
        HPController _controller = other.GetComponent<HPController>();
        if(_controller != null)
        {
            _controller.isCovered = false;
            other.GetComponent<Animator>().SetBool("IsCovered", false);
            if(regenerate != null)
            {
                StopCoroutine(regenerate);
            }
        }
    }

    void OnDisable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1);
        foreach(Collider nearbyObject in colliders)
        {
            HPController _controller = nearbyObject.GetComponent<HPController>();
            if(_controller != null)
            {
                _controller.isCovered = false;
                nearbyObject.GetComponent<Animator>().SetBool("IsCovered", false);
            }
        }
    }
}