using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelOutline : ScriptableRendererFeature
{
    [System.Serializable]
    public class SobelOutlineSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Range(0.0f, 1.0f), Tooltip("Edge-detection threshold.")]
        public float threshold = 0.5f;

        [Tooltip("Outline color.")]
        public Color outlineColor = Color.white;

        [Tooltip("Background color.")]
        public Color backgroundColor = Color.black;
    }

    public SobelOutlineSettings settings = new SobelOutlineSettings();

    class SobelOutlineRenderPass : ScriptableRenderPass
    {
        private Material material;

        public SobelOutlineSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/SobelOutline"));
        }

        public SobelOutlineRenderPass(string profilerTag)
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

            cmd.SetGlobalFloat("_Threshold", settings.threshold);
            cmd.SetGlobalColor("_OutlineColor", settings.outlineColor);
            cmd.SetGlobalColor("_BackgroundColor", settings.backgroundColor);
            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    SobelOutlineRenderPass pass;

    public override void Create()
    {
        pass = new SobelOutlineRenderPass("SobelOutline");
        name = "Sobel Outline";

        pass.settings = settings;
        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
