using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPlane : MonoBehaviour
{
    public Transform gunEnd;

    void Update()
    {
        transform.position = gunEnd.position - new Vector3(0, 0.01f, 0);
    }
}
