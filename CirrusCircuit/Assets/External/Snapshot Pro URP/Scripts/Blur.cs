using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Blur : ScriptableRendererFeature
{
    [System.Serializable]
    public class BlurSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Range(3, 27), Tooltip("Blur Strength")]
        public int strength = 5;
    }

    public BlurSettings settings = new BlurSettings();

    class BlurRenderPass : ScriptableRenderPass
    {
        private Material material;

        public BlurSettings settings;

        private int blurID;
        private RenderTargetIdentifier blurRT;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Blur"));
        }

        public BlurRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);

            int width = cameraTextureDescriptor.width;
            int height = cameraTextureDescriptor.height;

            blurID = Shader.PropertyToID("BlurRT");
            cmd.GetTemporaryRT(blurID, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

            blurRT = new RenderTargetIdentifier(blurID);

            ConfigureTarget(blurRT);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            cmd.SetGlobalInt("_KernelSize", settings.strength);
            cmd.SetGlobalFloat("_Spread", settings.strength / 7.5f);

            cmd.Blit(source, blurRT, material, 0);
            cmd.Blit(blurRT, source, material, 1);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    BlurRenderPass pass;

    public override void Create()
    {
        pass = new BlurRenderPass("Blur");
        name = "Blur";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
