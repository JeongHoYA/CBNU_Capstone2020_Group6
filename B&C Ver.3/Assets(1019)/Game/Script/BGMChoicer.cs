using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMChoicer : MonoBehaviour
{
    public AudioSource audioSource;
    
    public AudioClip[] audioclip;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("startBGM", 1f);
    }

    public void BGMChoice(int n)
    {
        audioSource.Stop();
        audioSource.clip = audioclip[n];
        audioSource.Play();
    }

    void startBGM()
    {
        audioSource.clip = audioclip[World.Instance.settings.BGMInspector];
        audioSource.Play();
    }
}
