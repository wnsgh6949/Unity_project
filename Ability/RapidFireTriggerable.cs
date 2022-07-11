using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RapidFireTriggerable : MonoBehaviour {

    ShootingController _shootingController;

    void Start()
    {
        _shootingController = GetComponent<ShootingController>();
    }

    public void Launch()
    {
        StartCoroutine(RapidFireMode());
    }

    IEnumerator RapidFireMode()
    {
        _shootingController.rapidFireMode = true;
        _shootingController.fireRate /= 2;
        yield return new WaitForSeconds(2.5f);
        _shootingController.rapidFireMode = false;
        _shootingController.fireRate *= 2;
    }
}