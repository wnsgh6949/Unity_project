using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] soundtrack;
    public AudioClip menuTheme;
    int previous, next;

    void Start()
    {
        AudioManager.instance.PlayMusic(menuTheme, 1);
        previous = soundtrack.Length;
    }

    public void PlayMusic()
    {
        while(true)
        {
            next = (int)Random.Range(0, 1000) % soundtrack.Length;
            if(next != previous)
            {
                break;
            }
        }
        AudioManager.instance.PlayMusic(soundtrack[next], 1);
        Invoke("PlayMusic", soundtrack[next].length);
    }
}
