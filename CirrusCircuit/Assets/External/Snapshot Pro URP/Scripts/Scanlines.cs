using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Scanlines : ScriptableRendererFeature
{
    [System.Serializable]
    public class ScanlinesSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Tooltip("Scanlines texture.")]
        public Texture2D scanlineTex = null;

        [Range(0f, 1f), Tooltip("Strength of the effect.")]
        public float strength = 1.0f;

        [Range(1, 64), Tooltip("Pixel size of the scanlines.")]
        public int size = 8;
    }

    public ScanlinesSettings settings = new ScanlinesSettings();

    class ScanlinesRenderPass : ScriptableRenderPass
    {
        private Material material;

        public ScanlinesSettings settings;
        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Scanlines"));
        }

        public ScanlinesRenderPass(string profilerTag)
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

            cmd.SetGlobalTexture("_ScanlineTex", settings.scanlineTex);
            cmd.SetGlobalFloat("_Strength", settings.strength);
            cmd.SetGlobalInt("_Size", settings.size);
            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    ScanlinesRenderPass pass;

    public override void Create()
    {
        pass = new ScanlinesRenderPass("Scanlines");
        name = "Scanlines";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
