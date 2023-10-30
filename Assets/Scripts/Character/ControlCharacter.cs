using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCharacter
{
    public string name;
    public string emotion;
    public string voice;
    public Vector2 position;
    public bool flip = false;

    public ControlCharacter(string name, string emotion, string voice, Vector2 position)
    {
        this.name = name;
        this.emotion = emotion;
        this.voice = voice;
        this.position = position;
    }

    
    public ControlCharacter(string name, string emotion, string voice, Vector2 position, bool flip)
    {
        this.name = name;
        this.emotion = emotion;
        this.voice = voice;
        this.position = position;
        this.flip = flip;
    }
}
