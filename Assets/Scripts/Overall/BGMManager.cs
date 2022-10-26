using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public AudioSource bgmAudioSource;
    public static BGMManager instance;

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

    void Start(){
    }

    public void ChangeBgm(string audioClipName) 
    {
        AudioClip audioClip = GameManager.LoadAudio(audioClipName);
        bgmAudioSource.clip = audioClip;

        bgmAudioSource.Play();
    }

}
