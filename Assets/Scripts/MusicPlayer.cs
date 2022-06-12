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
        }

        GameObject[] obj = GameObject.FindGameObjectsWithTag("Music");
        if(obj.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
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
    
    /// <summary>
    /// Plays a music clip with given clip
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySound(AudioClip clip, float volume)
    {
        src.PlayOneShot(clip, volume);
    }
}
