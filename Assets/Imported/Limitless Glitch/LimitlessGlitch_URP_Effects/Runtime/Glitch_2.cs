﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Glitch_2 : ScriptableRendererFeature
{
    Glitch2Pass GlitchPass;
    public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


    public override void Create()
    {
        GlitchPass = new Glitch2Pass(Event);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_2019 || UNITY_2020
        GlitchPass.Setup(renderer.cameraColorTarget);
#else

#endif
        renderer.EnqueuePass(GlitchPass);
    }
    public class Glitch2Pass : ScriptableRenderPass
    {
        static readonly string k_RenderTag = "Render Glitch2 Effect";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");

        static readonly int TempTargetId = Shader.PropertyToID("Glitch2");
        static readonly int _trashFrame1Id = Shader.PropertyToID("_trashFrame1");
        static readonly int _trashFrame2Id = Shader.PropertyToID("_trashFrame2");
        static readonly int _trashFrameId = Shader.PropertyToID("_trashFrame");
        static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        bool done = false;
        Limitless_Glitch2 Glitch2;
        Material Glitch2Material;
        RenderTargetIdentifier currentTarget;

        Texture2D _noiseTexture;


        public Glitch2Pass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find("LimitlessGlitch/Glitch_2");
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            Glitch2Material = CoreUtils.CreateEngineMaterial(shader);

        }
#if UNITY_2019 || UNITY_2020

#elif UNITY_2021
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			var renderer = renderingData.cameraData.renderer;
			currentTarget = renderer.cameraColorTarget;
		}
#else
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			var renderer = renderingData.cameraData.renderer;
			currentTarget = renderer.cameraColorTargetHandle;
		}
#endif

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Glitch2Material == null)
            {
                Debug.LogError("Material not created.");
                return;
            }

            var stack = VolumeManager.instance.stack;
            Glitch2 = stack.GetComponent<Limitless_Glitch2>();
            if (Glitch2 == null) { return; }
            if (!Glitch2.IsActive()) { return; }

            var cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Setup(in RenderTargetIdentifier currentTarget)
        {
            this.currentTarget = currentTarget;
        }

        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var source = currentTarget;
            int destination = TempTargetId;

            int _trashFrame1 = _trashFrame1Id;
            int _trashFrame2 = _trashFrame2Id;
            int _trashFrame = _trashFrameId;
            if (Glitch2.mask.value != null)
            {
                Glitch2Material.SetTexture(_Mask, Glitch2.mask.value);
                Glitch2Material.SetFloat(_FadeMultiplier, 1);
            }
            else
            {
                Glitch2Material.SetFloat(_FadeMultiplier, 0);
            }
            int shaderPass = 0;

            cmd.GetTemporaryRT(_trashFrame1, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.GetTemporaryRT(_trashFrame2, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);

            if(!done)
                SetUpResources(Glitch2.resolutionMultiplier.value);

            if (UnityEngine.Random.value > Mathf.Lerp(0.9f, 0.5f, Glitch2.speed.value))
            {
                SetUpResources(Glitch2.resolutionMultiplier.value);
                UpdateNoiseTexture(Glitch2.resolutionMultiplier.value);
            }

            // Update trash frames.
            int fcount = Time.frameCount;

            if (fcount % 13 == 0) cmd.Blit(source, _trashFrame1);
            if (fcount % 73 == 0) cmd.Blit(source, _trashFrame2);

            _trashFrame = UnityEngine.Random.value > 0.5f ? _trashFrame1 : _trashFrame2;

            
            Glitch2Material.SetFloat("_Intensity", Glitch2.amount.value);
            Glitch2Material.SetFloat("minBlur", 2);
            Glitch2Material.SetFloat("_ColorIntensity", Glitch2.intensity.value);

            if (_noiseTexture == null)
            {
                UpdateNoiseTexture(Glitch2.resolutionMultiplier.value);
            }

            Glitch2Material.SetTexture("_NoiseTex", _noiseTexture);

            cmd.SetGlobalTexture("_TrashTex", _trashFrame);
            cmd.SetGlobalTexture(MainTexId, source);

            cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);


            cmd.Blit(source, destination);
            cmd.Blit(destination, source, Glitch2Material, shaderPass);
        }
        void SetUpResources(float g_2Res)
        {

            if (done)
                return;
            Vector2Int texVec = new Vector2Int((int)(g_2Res * 64), (int)(g_2Res * 62));
            _noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false)
            {

                hideFlags = HideFlags.DontSave,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };

            UpdateNoiseTexture(g_2Res);
            done = true;
        }
        void UpdateNoiseTexture(float g_2Res)
        {
            Color color = RandomColor();
            if (_noiseTexture == null)
            {
                Vector2Int texVec = new Vector2Int((int)(g_2Res * 64), (int)(g_2Res * 32));
                _noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false);
            }
            for (var y = 0; y < _noiseTexture.height; y++)
            {
                for (var x = 0; x < _noiseTexture.width; x++)
                {
                    if (UnityEngine.Random.value > Glitch2.stretchMultiplier.value) color = RandomColor();
                    _noiseTexture.SetPixel(x, y, color);
                }
            }

            _noiseTexture.Apply();
        }
        static Color RandomColor()
        {
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }
    }

}


