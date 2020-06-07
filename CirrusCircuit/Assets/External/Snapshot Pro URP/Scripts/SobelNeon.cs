using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelNeon : ScriptableRendererFeature
{
    [System.Serializable]
    public class SobelNeonSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Range(0.0f, 1.0f), Tooltip("Saturation values lower than this will be clamped to this.")]
        public float saturationFloor = 0.75f;

        [Range(0.0f, 1.0f), Tooltip("Lightness/value values lower than this will be clamped to this.")]
        public float lightnessFloor = 0.75f;
    }

    public SobelNeonSettings settings = new SobelNeonSettings();

    class SobelNeonRenderPass : ScriptableRenderPass
    {
        private Material material;

        public SobelNeonSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/SobelNeon"));
        }

        public SobelNeonRenderPass(string profilerTag)
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

            cmd.SetGlobalFloat("_SaturationFloor", settings.saturationFloor);
            cmd.SetGlobalFloat("_LightnessFloor", settings.lightnessFloor);
            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    SobelNeonRenderPass pass;

    public override void Create()
    {
        pass = new SobelNeonRenderPass("SobelNeon");
        name = "Sobel Neon";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
