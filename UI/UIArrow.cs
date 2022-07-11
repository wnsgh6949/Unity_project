using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIArrow : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime * 10);
    }

    void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}
