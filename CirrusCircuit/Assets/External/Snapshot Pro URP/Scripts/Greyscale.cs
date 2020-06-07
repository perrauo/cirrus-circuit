using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Greyscale : ScriptableRendererFeature
{
    [System.Serializable]
    public class GreyscaleSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Range(0f, 1f), Tooltip("Greyscale effect intensity.")]
        public float strength = 1.0f;
    }

    public GreyscaleSettings settings = new GreyscaleSettings();

    class GreyscaleRenderPass : ScriptableRenderPass
    {
        private Material material;

        public GreyscaleSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Greyscale"));
        }

        public GreyscaleRenderPass(string profilerTag)
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

            cmd.SetGlobalFloat("_Strength", settings.strength);
            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    GreyscaleRenderPass pass;

    public override void Create()
    {
        pass = new GreyscaleRenderPass("Greyscale");
        name = "Greyscale";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
