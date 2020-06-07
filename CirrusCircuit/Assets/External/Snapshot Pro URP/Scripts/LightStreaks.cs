using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightStreaks : ScriptableRendererFeature
{
    [System.Serializable]
    public class LightStreaksSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Range(3, 500), Tooltip("Blur Strength.")]
        public int strength = 250;

        [Range(0.0f, 25.0f), Tooltip("Luminance Threshold - pixels above this luminance will glow.")]
        public float luminanceThreshold = 10.0f;
    }

    public LightStreaksSettings settings = new LightStreaksSettings();

    class LightStreaksRenderPass : ScriptableRenderPass
    {
        private Material material;

        public LightStreaksSettings settings;

        private int blurID;
        private RenderTargetIdentifier blurRT;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/LightStreaks"));
        }

        public LightStreaksRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);

            int width = cameraTextureDescriptor.width / 4;
            int height = cameraTextureDescriptor.height / 4;

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
            cmd.SetGlobalFloat("_LuminanceThreshold", settings.luminanceThreshold);

            cmd.Blit(source, blurRT, material, 0);

            cmd.SetGlobalTexture("_BlurTex", blurRT);

            cmd.Blit(source, source, material, 1);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    LightStreaksRenderPass pass;

    public override void Create()
    {
        pass = new LightStreaksRenderPass("LightStreaks");
        name = "Light Streaks";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
