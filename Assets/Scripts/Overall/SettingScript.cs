using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingScript : MonoBehaviour
{
    [HideInInspector] int voice_char_num = 1;

    public void VoiceScrollChange_CharNum(int char_num)
    {
        voice_char_num = char_num;
    }
    public void VoiceScrollChange(Scrollbar s)
    {
        float volume = s.value;
        switch (voice_char_num)
        {
            case 1:
                AudioManager.instance.char1_voice = volume;
                break;
            case 2:
                AudioManager.instance.char2_voice = volume;
                break; 
            case 3:
                AudioManager.instance.char3_voice = volume;
                break; 
            default:
                AudioManager.instance.etc_voice = volume;
                break;
        }
        
    }

    public void AudioScrollChange(Scrollbar s)
    {
        AudioManager.instance.audioSource.volume=s.value;
    }

    public void BGMScrollChange(Scrollbar s)
    {
        AudioManager.instance.bgmAudioSource.volume = s.value;
    }

    public void TextSpeedScrollChange(Scrollbar s)
    {
        float value = s.value;
        if (value > 1)
            value = 1;
        if (value < 0)
            value = 0;


        //interpolation, value = 1 -> speed = 0, value = 0 -> speed = 0.1
        GameManager.instance.TextSpeed = (1.0f - value) * 0.1f;

    }
}
