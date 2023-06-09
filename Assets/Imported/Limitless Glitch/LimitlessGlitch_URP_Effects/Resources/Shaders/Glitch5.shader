﻿Shader "LimitlessGlitch/Glitch5" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
    HLSLINCLUDE

	 #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

		TEXTURE2D(_MainTex);
		SAMPLER(sampler_MainTex);
		TEXTURE2D(_Mask);
		SAMPLER(sampler_Mask);
		float screenLinesNum = 240.0;
		half strength = 1;
		float noiseLinesNum = 240.0;
		float noiseQuantizeX = 1.0; 
		float random(float2 c) 
        {
			return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
		}
		float mod(float x, float y)
		{
			return x - y * floor(x / y);
		}
		float jitterHFreq = 0.5;
		float jitterHRate = 0.5;
		#pragma shader_feature SHIFT_H
		#pragma shader_feature CUSTOM_INTERVAL
		float jitterHAmount = 0.5; 
		float2 JitterHRandSeed;
		half speed;
		float time_ = 0.0;
		float SLN = 0.0; 
		float SLN_Noise = 0.0; 
		float ONE_X = 0.0; 
		float ONE_Y = 0.0; 
		float _FadeMultiplier;

		float onOff(float a, float b, float c, float t)
		{
			return step(c, sin(t + a*cos(t*b)));
		}
		half3 rgb2yiq(half3 c)
		{   
			return half3(
						(0.2989*c.x + 0.5959*c.y + 0.2115*c.z),
						(0.5870*c.x - 0.2744*c.y - 0.5229*c.z),
						(0.1140*c.x - 0.3216*c.y + 0.3114*c.z)
						);
		};
		half3 yiq2rgb(half3 c)
		{				
			return half3(
						(	 1.0*c.x +	  1.0*c.y + 	1.0*c.z),
						( 0.956*c.x - 0.2720*c.y - 1.1060*c.z),
						(0.6210*c.x - 0.6474*c.y + 1.7046*c.z)
						);
		};

        float4 FragV(Varyings i) : SV_Target
        {
			float t = time_;			
			float2 p = i.uv.xy;
			half strength = 1.0;
			ONE_X = 1.0/_ScreenParams.x; 

			#if CUSTOM_INTERVAL
				strength = 1;
			#else
				strength = smoothstep(jitterHFreq * jitterHRate, jitterHFreq, jitterHFreq - mod(_Time.y, jitterHFreq));
			#endif

				if (_FadeMultiplier > 0)
				{
					float alpha_Mask = step(0.0001, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.uv).r);
					strength *= alpha_Mask;
				}
			   	if( fmod( p.x * screenLinesNum, 2)<1.0)
				{
					#if SHIFT_H				    		
						p.x += ONE_X*cos(t*speed*100.0)*jitterHAmount*strength;
					#else
						p.y += ONE_X*cos(t*speed*100.0)*jitterHAmount*strength;
					#endif
				} 

			half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, p);
			half3 signal = rgb2yiq(col.rgb);
			col.rgb = yiq2rgb(signal);
			return col; 
        }

		float4 FragH(Varyings i) : SV_Target
        {
			float t = time_;			
			float2 p = i.uv.xy;
			half strength = 1.0;
			ONE_X = 1.0/_ScreenParams.x; 

			#if CUSTOM_INTERVAL
				strength = 1;
			#else
				strength = smoothstep(jitterHFreq * jitterHRate, jitterHFreq, jitterHFreq - mod(_Time.y, jitterHFreq));
			#endif
				if (_FadeMultiplier > 0)
				{
					float alpha_Mask = step(0.0001, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.uv).r);
					strength *= alpha_Mask;
				}
			   	if( fmod( p.y * screenLinesNum, 2)<1.0)
				{
					#if SHIFT_H				    		
						p.x += ONE_X*cos(t*speed*100.0)*jitterHAmount*strength;
					#else
						p.y += ONE_X*cos(t*speed*100.0)*jitterHAmount*strength;
					#endif
				} 
			half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, p);
			half3 signal = rgb2yiq(col.rgb);
			col.rgb = yiq2rgb(signal);
			return half4(col); 
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex Vert
                #pragma fragment FragH

            ENDHLSL
        }
		        Pass
        {
            HLSLPROGRAM

                #pragma vertex Vert
                #pragma fragment FragV

            ENDHLSL
        }
    }
}