Shader "Unlit/FadeShader"
{
    
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTex] _BaseMap("Base Map", 2D) = "white"{}
        _FadeLine("Fade Line", Range(0, 1)) = 0
        _Smoothness("Scalar", Range(0, 20)) = 5
        _Option("option", Integer) = 0
        
    }

    // The SubShader block containing the Shader code. 
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags {            
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader. 
            #pragma vertex vert
            // This line defines the name of the fragment shader. 
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;  
                float4 color        : COLOR0;
                float2 uv           : TEXCOORD0;               
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                float4 color        : COLOR0;
                float2 uv           : TEXCOORD0;
            };            

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float _Smoothness;
                float _FadeLine;
                float _Option;
            CBUFFER_END
            
            
            // The vertex shader definition with properties defined in the Varyings 
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous space
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                // Returning the output.
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                
                OUT.color = IN.color;
                return OUT;
            }

            // The fragment shader definition.            
            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                
                [branch]switch(_Option){
                    case 0 : {
                        color.a = 1 - _FadeLine;
                        break;
                    }
                    case 1 : {
                        color.a = saturate((_Smoothness*IN.uv.x)+1-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 2 : {
                        color.a = saturate((-1*_Smoothness*IN.uv.x)+(_Smoothness+1)-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 3 : {
                        color.a = saturate((_Smoothness*IN.uv.y)+1-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 4 : {
                        color.a = saturate((-1*_Smoothness*IN.uv.y)+(_Smoothness+1)-(_FadeLine*(_Smoothness+1)));
                        break;
                    }
                    case 5 : {
                        color.a = saturate((sqrt(pow(IN.uv.x - 0.5, 2) + pow(IN.uv.y - 0.5, 2)) * _Smoothness) - (_Smoothness * _FadeLine) + (1-_FadeLine));
                        break;
                    }
                    default : {
                        color.a = 1 - _FadeLine;
                        break;
                    }
                }

                color *= IN.color;
                return color;
            }
            ENDHLSL
        }
    }
}