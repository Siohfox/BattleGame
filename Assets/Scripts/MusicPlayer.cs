using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    AudioSource src;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    private void Start()
    {
        src = gameObject.GetComponent<AudioSource>();
    }

    public void StopPlayMusic()
    {
        if(src.isPlaying)
        {
            src.Stop();
        }    
        else
        {
            src.Play();
        }
    }
}
