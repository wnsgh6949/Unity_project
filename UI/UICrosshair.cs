using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICrosshair : MonoBehaviour
{
    GameObject hitMark;

    void Start()
    {
        hitMark = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void OnHit()
    {
        hitMark.SetActive(true);
        Invoke("OnHitAfter", 0.05f);
    }

    void OnHitAfter()
    {
        hitMark.SetActive(false);
    }
}