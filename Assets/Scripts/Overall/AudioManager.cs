using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmAudioSource;
    public AudioSource audioSource;
    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
    }

    public void ChangeBgm(string audioClipName)
    {
        AudioClip audioClip = GameManager.LoadAudio(audioClipName);
        bgmAudioSource.clip = audioClip;

        bgmAudioSource.Play();
    }

    public void PlayAudio(string audioClipName)
    {
        AudioClip audioClip = GameManager.LoadAudio(audioClipName);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

}
