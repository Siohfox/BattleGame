using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    AudioSource src;

    public static SfxPlayer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        GameObject[] obj = GameObject.FindGameObjectsWithTag("SFX");
        if (obj.Length > 1)
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

    /// <summary>
    /// Plays a music clip with given clip
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySound(AudioClip clip, float volume)
    {
        src.PlayOneShot(clip, volume);
    }
}
