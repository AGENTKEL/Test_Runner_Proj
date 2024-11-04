using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> Sounds;
    public AudioSource audioSource;
    public int pos;

    public List<AudioClip> Sounds2;
    public AudioSource audioSource2;
    public int pos2;

    public List<AudioClip> Sounds3;
    public AudioSource audioSource3;
    public int pos3;
    
    public List<AudioClip> Sounds4;
    public AudioSource audioSource4;
    public int pos4;

    public void PlaySound()
    {
        pos = (int)Mathf.Floor(Random.Range(0, Sounds.Count));
        audioSource.PlayOneShot(Sounds[pos]);
    }

    public void PlaySound2()
    {
        pos2 = (int)Mathf.Floor(Random.Range(0, Sounds2.Count));
        audioSource2.PlayOneShot(Sounds2[pos2]);
    }

    public void PlaySound3()
    {
        pos3 = (int) Mathf.Floor(Random.Range(0, Sounds3.Count));
        audioSource3.PlayOneShot(Sounds3[pos3]);
    }
    
    public void PlaySound4()
    {
        pos4 = (int) Mathf.Floor(Random.Range(0, Sounds4.Count));
        audioSource4.PlayOneShot(Sounds4[pos4]);
    }
}
