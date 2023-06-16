Shader "Unlit/FadeShader"
{

    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        // [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        // [MainTex] _BaseMap("Base Map", 2D) = "white"{}
        _FadeLine("Fade Line", Range(0, 1)) = 0
        _Smoothness("Scalar", Range(0, 20)) = 5
        _Option("option", Integer) = 0
        
    }

        // The SubShader block containing the Shader code. 

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            #pragma multi_compile _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                half4   color       : COLOR;
                float2  uv          : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _MainTex_ST;
                float4 _Color;
                float _Smoothness;
                float _FadeLine;
                float _Option;
            CBUFFER_END

            Varyings UnlitVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(v.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            half4 UnlitFragment(Varyings i) : SV_Target
            {
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_DATA_2D(inputData, i.positionWS);

                if(CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                
                [branch]switch(_Option){
                    case 0 : {
                        mainTex.a = 1 - _FadeLine;
                        break;
                    }
                    case 1 : {
                        mainTex.a = saturate((_Smoothness*i.uv.x)+1-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 2 : {
                        mainTex.a = saturate((-1*_Smoothness*i.uv.x)+(_Smoothness+1)-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 3 : {
                        mainTex.a = saturate((_Smoothness*i.uv.y)+1-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 4 : {
                        mainTex.a = saturate((-1*_Smoothness*i.uv.y)+(_Smoothness+1)-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 5 : {
                        mainTex.a = saturate((sqrt(pow(i.uv.x - 0.5, 2) + pow(i.uv.y - 0.5, 2)) * _Smoothness) - (_Smoothness * _FadeLine) + (1-_FadeLine));
                        break;
                    }
                    default : {
                        mainTex.a = 1 - _FadeLine;
                        break;
                    }
                }
                mainTex = i.color * mainTex;
                return mainTex;
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                float4  color           : COLOR;
                float2  uv              : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS      : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                half4 _MainTex_ST;
                float4 _Color;
                float _Smoothness;
                float _FadeLine;
                float _Option;
            CBUFFER_END

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(attributes.positionOS);
                #endif
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.color = attributes.color;
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_DATA_2D(inputData, i.positionWS);

                if(CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                
                [branch]switch(_Option){
                    case 0 : {
                        mainTex.a = 1 - _FadeLine;
                        break;
                    }
                    case 1 : {
                        mainTex.a = saturate((_Smoothness*i.uv.x)+1-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 2 : {
                        mainTex.a = saturate((-1*_Smoothness*i.uv.x)+(_Smoothness+1)-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 3 : {
                        mainTex.a = saturate((_Smoothness*i.uv.y)+1-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 4 : {
                        mainTex.a = saturate((-1*_Smoothness*i.uv.y)+(_Smoothness+1)-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 5 : {
                        mainTex.a = saturate((sqrt(pow(i.uv.x - 0.5, 2) + pow(i.uv.y - 0.5, 2)) * _Smoothness) - (_Smoothness * _FadeLine) + (1-_FadeLine));
                        break;
                    }
                    default : {
                        mainTex.a = 1 - _FadeLine;
                        break;
                    }
                }

                return mainTex;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}

