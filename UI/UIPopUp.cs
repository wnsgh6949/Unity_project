using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopUp : MonoBehaviour
{
    Text text;

    void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine(Fade());
    }

    void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
        transform.position += Vector3.up * 0.01f;
    }

    IEnumerator Fade()
    {
        for(float i=1; i>=0; i-=Time.deltaTime*2)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, i);
            yield return null;
        }
        Destroy(gameObject);
    }
}