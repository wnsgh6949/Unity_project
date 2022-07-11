using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealTimer : MonoBehaviour
{
    public GameObject uiPrefab;
    public Transform target;
    public Transform ui;

    Transform cam;
    Image timerImage;
    Image mask;
    
    float coolDownDuration = 5;
    float coolDownTimeLeft;

    void Awake()
    {
        cam = Camera.main.transform;

        foreach(Canvas c in FindObjectsOfType<Canvas>())
        {
            if(c.renderMode == RenderMode.WorldSpace)
            {
                ui = Instantiate(uiPrefab, c.transform).transform;
                timerImage = ui.GetComponent<Image>();
                mask = ui.GetChild(0).GetComponent<Image>();
                break;
            }
        }
    }

    void LateUpdate()
    {
        ui.position = target.position + new Vector3(-0.4f, 0.1f, 0);
        ui.forward = -cam.forward;
    }

    public IEnumerator CoolDown()
    {
        coolDownTimeLeft = 5;
        mask.fillAmount = 1;
        while(coolDownTimeLeft > 0)
        {
            coolDownTimeLeft -= Time.deltaTime;
            mask.fillAmount = (coolDownTimeLeft / coolDownDuration);
            yield return null;
        }
    }
}
