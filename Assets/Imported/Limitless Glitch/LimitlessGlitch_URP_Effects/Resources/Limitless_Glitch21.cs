﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[VolumeComponentMenu("Limitless Glitch/Glitch21")]
public class Limitless_Glitch21 : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);

    public ClampedFloatParameter range = new ClampedFloatParameter(0.05f, 0f, 1f);
    public ClampedFloatParameter noiseQuality = new ClampedFloatParameter(250f,0f, 505f);
    public ClampedFloatParameter noiseIntensity = new ClampedFloatParameter(0.0088f, 0f, 1f);
    public ClampedFloatParameter offsetIntensity = new ClampedFloatParameter(0.02f, 0f, 1f);
    public ClampedFloatParameter colorOffsetIntensity = new ClampedFloatParameter(1.3f, 0f, 10f);


    public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}