using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Pixelate : ScriptableRendererFeature
{
    [System.Serializable]
    public class PixelateSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Range(1, 128), Tooltip("Size of each new 'pixel' in the image.")]
        public int pixelSize = 3;
    }

    public PixelateSettings settings = new PixelateSettings();

    class PixelateRenderPass : ScriptableRenderPass
    {
        public PixelateSettings settings;

        private int pixelID;
        private RenderTargetIdentifier pixelRT;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public PixelateRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);

            int width = cameraTextureDescriptor.width / settings.pixelSize;
            int height = cameraTextureDescriptor.height / settings.pixelSize;

            pixelID = Shader.PropertyToID("PixelRT");
            cmd.GetTemporaryRT(pixelID, width, height, 0, FilterMode.Point, RenderTextureFormat.ARGB32);

            pixelRT = new RenderTargetIdentifier(pixelID);

            ConfigureTarget(pixelRT);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            cmd.Blit(source, pixelRT);
            cmd.Blit(pixelRT, source);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    PixelateRenderPass pass;

    public override void Create()
    {
        pass = new PixelateRenderPass("Pixelate");
        name = "Pixelate";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
