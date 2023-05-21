using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioSource AudioSource { get; private set; }
    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        if(instance == null)
        {
            instance = this;
        }    
    }

    public static void PlayAudio(AudioClip clip, float volumeScale=1f)
    {
        instance.AudioSource.PlayOneShot(clip, volumeScale);
    }

    public static void PlayAudioAt(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        AudioSource.PlayClipAtPoint(clip, position, volumeScale);
    }
}
