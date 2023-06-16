﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Limitless Glitch/Glitch17")]
public class LimitlessGlitch17 : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
    [Tooltip("Stops effect without disabling it. Use it in your effect manipulation script")]
    public BoolParameter stop = new BoolParameter(false);
    [Tooltip("Effect strength")]
    public ClampedFloatParameter strength = new ClampedFloatParameter(0.03f, 0, 0.5f);
    [Tooltip("Effect speed")]
    public ClampedFloatParameter speed = new ClampedFloatParameter(16f, 0, 100f);
    [Tooltip("Effect size")]
    public ClampedFloatParameter size = new ClampedFloatParameter(1f, 0, 10f);
    [Tooltip("Effect Fade")]
    public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0, 1f);
    [Tooltip("RGBSplit")]
    public ClampedFloatParameter RGBSplit = new ClampedFloatParameter(1f, 0, 2f);
    [Tooltip("Random effect activation for short period of time. 0  = never activate effect, 1 = endless effect")]
    public ClampedFloatParameter randomActivateAmount = new ClampedFloatParameter(1f, 0, 1);
    [Space]
    [Tooltip("Mask texture")]
    public TextureParameter mask = new TextureParameter(null);

    public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;

}