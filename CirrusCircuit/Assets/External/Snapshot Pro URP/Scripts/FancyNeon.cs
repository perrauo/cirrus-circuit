using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FancyNeon : ScriptableRendererFeature
{
    [System.Serializable]
    public class FancyNeonSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Range(0.0f, 1.0f), Tooltip("Threshold for colour-based edge detection.")]
        public float colorSensitivity = 0.1f;

        [Range(0.0f, 1.0f), Tooltip("Strength of colour-based edges.")]
        public float colorStrength = 0.5f;

        [Range(0.0f, 1.0f), Tooltip("Threshold for depth-based edge detection.")]
        public float depthSensitivity = 0.01f;

        [Range(0.0f, 1.0f), Tooltip("Strength of depth-based edges.")]
        public float depthStrength = 0.75f;

        [Range(0.0f, 1.0f), Tooltip("Threshold for normal-based edge detection.")]
        public float normalSensitivity = 0.1f;

        [Range(0.0f, 1.0f), Tooltip("Strength of normal-based edges.")]
        public float normalStrength = 0.75f;

        [Tooltip("Pixels past this depth threshold will not be edge-detected.")]
        public float depthThreshold = 0.99f;

        [Range(0.0f, 1.0f), Tooltip("Saturation values lower than this will be clamped to this.")]
        public float saturationFloor = 0.75f;

        [Range(0.0f, 1.0f), Tooltip("Lightness/value values lower than this will be clamped to this.")]
        public float lightnessFloor = 0.75f;
    }

    public FancyNeonSettings settings = new FancyNeonSettings();

    class FancyNeonRenderPass : ScriptableRenderPass
    {
        private Material material;

        public FancyNeonSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Neon"));
        }

        public FancyNeonRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
            
            cmd.SetGlobalFloat("_ColorSensitivity", settings.colorSensitivity);
            cmd.SetGlobalFloat("_ColorStrength", settings.colorStrength);
            cmd.SetGlobalFloat("_DepthSensitivity", settings.depthSensitivity);
            cmd.SetGlobalFloat("_DepthStrength", settings.depthStrength);
            cmd.SetGlobalFloat("_NormalsSensitivity", settings.normalSensitivity);
            cmd.SetGlobalFloat("_NormalsStrength", settings.normalStrength);
            cmd.SetGlobalFloat("_DepthThreshold", settings.depthThreshold);
            cmd.SetGlobalFloat("_SaturationFloor", settings.saturationFloor);
            cmd.SetGlobalFloat("_LightnessFloor", settings.lightnessFloor);

            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    DepthNormalsPass depthNormalsPass;
    FancyNeonRenderPass pass;

    public override void Create()
    {
        depthNormalsPass = new DepthNormalsPass();

        pass = new FancyNeonRenderPass("FancyNeon");
        name = "Fancy Neon";

        pass.settings = settings;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        depthNormalsPass.Setup(renderingData.cameraData.cameraTargetDescriptor);
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(depthNormalsPass);
        renderer.EnqueuePass(pass);
    }
}
