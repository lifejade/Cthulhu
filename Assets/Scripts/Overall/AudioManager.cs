using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmAudioSource;
    public AudioSource audioSource;
    public AudioSource voiceSource;
    public static AudioManager instance;

    [HideInInspector] public float char1_voice = 1.0f;
    [HideInInspector] public float char2_voice = 1.0f;
    [HideInInspector] public float char3_voice = 1.0f;
    [HideInInspector] public float etc_voice = 1.0f;


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

    public void PlayVoice(string audioClipName, int character_num = 4)
    {
        AudioClip audioClip = GameManager.LoadAudio(audioClipName);
        voiceSource.clip = audioClip;

        switch (character_num)
        {
            case 1:
                voiceSource.volume = char1_voice;
                break;
            case 2:
                voiceSource.volume = char2_voice;
                break;
            case 3:
                voiceSource.volume = char3_voice;
                break;

            default:
                voiceSource.volume = etc_voice;
                break;
        }
       
        voiceSource.Play();

    }

}
