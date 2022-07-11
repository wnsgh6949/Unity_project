using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    CinemachineVirtualCamera vCam;
    CinemachineComponentBase compBase;
    CinemachineCameraOffset offset;

    public GameObject canvas1, canvas2;
    
    Coroutine rotation;
    float camDistance;
    float totalChange = 0f;
    float maxDistance = 1.5f;

    void Start()
    {
        vCam = FindObjectOfType<CinemachineVirtualCamera>();
        compBase = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        offset = vCam.GetComponent<CinemachineCameraOffset>();

        rotation = StartCoroutine(CameraRotation());
    }

    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            camDistance = Input.GetAxis("Mouse ScrollWheel") * 5f;
            if(compBase is CinemachineFramingTransposer)
            {
                if(Mathf.Abs(totalChange - camDistance) <= maxDistance)
                {
                    (compBase as CinemachineFramingTransposer).m_CameraDistance -= camDistance;
                    totalChange -= camDistance;
                }
            }
        }
    }
    
    IEnumerator CameraRotation()
    {
        while(true)
        {
            transform.RotateAround(Vector3.zero, Vector3.up, Time.unscaledDeltaTime);
            yield return null;
        }
    }

    public void StopRotation()
    {
        if(rotation != null)
        {
            StopCoroutine(rotation);
        }
    }

    public IEnumerator CameraShake(float intensity)
    {
        float recoilTime = intensity / 2;
        float percent = 0;

        Vector3 recoil = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));
        while(percent < recoilTime)
        {
            percent += Time.deltaTime * 3;
            offset.m_Offset += recoil * Time.deltaTime * 3;
            yield return null;
        }
        while(percent > 0)
        {
            percent -= Time.deltaTime;
            offset.m_Offset -= recoil * Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
