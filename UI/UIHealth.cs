using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    public GameObject uiPrefab;
    public Transform target;
    public Transform ui;

    Transform cam;
    Image healthSlider_1;
    Image healthSlider_2;

    void Awake()
    {
        cam = Camera.main.transform;

        foreach(Canvas c in FindObjectsOfType<Canvas>())
        {
            if(c.renderMode == RenderMode.WorldSpace)
            {
                ui = Instantiate(uiPrefab, c.transform).transform;
                healthSlider_1 = ui.GetChild(0).GetComponent<Image>();
                healthSlider_2 = ui.GetChild(1).GetComponent<Image>();
                break;
            }
        }
    }

    void LateUpdate()
    {
        ui.position = target.position;
        ui.forward = -cam.forward;
    }

    public IEnumerator SetValue(float value)
    {
        healthSlider_2.fillAmount = value;
        for(float i=healthSlider_1.fillAmount; i>=value; i-=Time.deltaTime*4)
        {
            healthSlider_1.fillAmount = i;
            yield return null;
        }
    }
}
