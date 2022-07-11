using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour
{
    Volume _volume;
    Vignette _vignette;
    HPController _playerHP;

    void Start()
    {
        _volume = GetComponent<Volume>();
        _volume.profile.TryGet(out _vignette);
        _playerHP = GameObject.FindWithTag("Player").GetComponent<HPController>();
    }

    void Update()
    {
        if(_playerHP.currentHealth < 50f)
        {
            _vignette.color.value = new Color(2 - _playerHP.currentHealth/50f, 0, 0);
        }
        else
        {
            _vignette.color.value = Color.black;
        }
    }
}
