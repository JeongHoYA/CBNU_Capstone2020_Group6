using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMChoicer : MonoBehaviour
{

    public byte trackSelector; // 현재 값

    byte trackIndicator; // 이전 값

    public AudioSource audioSource;
    
    public AudioClip[] audioclip;
    // Start is called before the first frame update
    void Start()
    {
        trackIndicator = trackSelector = 0;

        audioSource.clip = audioclip[trackSelector];
        audioSource.Play();
    }

    public void BGMChoice(int n)
    {
        audioSource.Stop();
        audioSource.clip = audioclip[n];
        audioSource.Play();
    }
}
