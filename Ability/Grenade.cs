using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Grenade : MonoBehaviour
{
    CameraMovement _cameraMovement;
    Rigidbody _rigidbody;

    public GameObject explosionEffect;
    public GameObject destructionEffect;
    public Transform thrower;
    public Vector3 velocity;

    public float radius = 1.5f;
    public int damage = 100;


    void Awake()
    {
        _cameraMovement = FindObjectOfType<CinemachineVirtualCamera>().GetComponent<CameraMovement>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        AudioManager.instance.PlaySound("Explosion", transform.position);
        Destroy(Instantiate(explosionEffect, transform.position, Quaternion.identity) as GameObject, 2);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        StartCoroutine(CameraShake());
        foreach(Collider nearbyObject in colliders)
        {
            if(nearbyObject.tag == "Cover")
            {
                Destroy(Instantiate(destructionEffect, nearbyObject.transform.position, Quaternion.identity) as GameObject, 2);
                //nearbyObject.transform.GetChild(0).gameObject.SetActive(false);
                //nearbyObject.transform.GetChild(1).gameObject.SetActive(false);
                Destroy(nearbyObject.gameObject);
            }
            else
            {
                HPController hpController = nearbyObject.GetComponent<HPController>();
                if(hpController != null)
                {
                    hpController.Damage(damage);
                }
            }
        }
        Destroy(GetComponent<Renderer>());
        Destroy(gameObject, 2);
        Destroy(this);
    }

    IEnumerator CameraShake()
    {
        StartCoroutine(_cameraMovement.CameraShake(0.5f));
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(_cameraMovement.CameraShake(0.4f));
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(_cameraMovement.CameraShake(0.3f));
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(_cameraMovement.CameraShake(0.2f));
    }
}
