using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool fromPlayer;
    float speed = 35f;
    float lifetime = 1f;
    int damage = 5;

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update() {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(ray, out RaycastHit hit, moveDistance))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        Debug.Log(hit.collider.name);
        HPController damageableObject = hit.collider.GetComponent<HPController>();
        if(damageableObject != null)
        {
            damageableObject.Damage(damage);
            if(fromPlayer && damageableObject.tag == "Enemy")
            {
                FindObjectOfType<UICrosshair>().OnHit();
            }
        }
        Destroy(gameObject);
    }
}
