using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAmmoBar : MonoBehaviour
{
    public Image mag;
    public Image mask;
    public Image ammoBar;

    float originalSize;
    Vector2 magSize;
    Vector2 maskSize;

    void Start()
    {
        originalSize = ammoBar.rectTransform.rect.height;
        mag.color = new Color(mag.color.r, mag.color.g, mag.color.b, 0);
        ammoBar.color = new Color(ammoBar.color.r, ammoBar.color.g, ammoBar.color.b, 0);
        magSize = new Vector2(mag.rectTransform.rect.width, mag.rectTransform.rect.height);
        maskSize = new Vector2(mask.rectTransform.rect.width, mask.rectTransform.rect.height);
    }
    
    void Update()
    {
        mag.transform.position = Input.mousePosition + new Vector3(-25f, 0f, 0f);
    }

    public void SetValue(float value)
    {
        ammoBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalSize * value);
    }

    public IEnumerator FadeUI(bool fadeAway)
    {
        if(fadeAway)
        {
            for(float i=0.75f; i>=0; i-=Time.deltaTime*4)
            {
                mag.color = new Color(mag.color.r, mag.color.g, mag.color.b, i);
                ammoBar.color = new Color(ammoBar.color.r, ammoBar.color.g, ammoBar.color.b, i);
                mag.rectTransform.sizeDelta = magSize * (0.25f + i);
                mask.rectTransform.sizeDelta = maskSize * (0.25f + i);
                yield return null;
            }
        }
        else
        {
            for(float i=0; i<=0.75f; i+=Time.deltaTime*4)
            {
                mag.color = new Color(mag.color.r, mag.color.g, mag.color.b, i);
                ammoBar.color = new Color(ammoBar.color.r, ammoBar.color.g, ammoBar.color.b, i);
                mag.rectTransform.sizeDelta = magSize * (0.25f + i);
                mask.rectTransform.sizeDelta = maskSize * (0.25f + i);
                yield return null;
            }
        }
    }
}