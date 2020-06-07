using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Glitch : ScriptableRendererFeature
{
    [System.Serializable]
    public class GlitchSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Tooltip("Texture which controls the strength of the glitch offset based on y-coordinate.")]
        public Texture2D offsetTexture;

        [Range(0f, 5.0f), Tooltip("Glitch effect intensity.")]
        public float offsetStrength = 0.1f;

        [Range(0.0f, 25.0f), Tooltip("Controls how many times the glitch texture repeats vertically.")]
        public float verticalTiling = 5.0f;
    }

    public GlitchSettings settings = new GlitchSettings();

    class GlitchRenderPass : ScriptableRenderPass
    {
        private Material material;

        public GlitchSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Glitch"));
        }

        public GlitchRenderPass(string profilerTag)
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

            cmd.SetGlobalTexture("_OffsetTex", settings.offsetTexture);
            cmd.SetGlobalFloat("_OffsetStrength", settings.offsetStrength);
            cmd.SetGlobalFloat("_VerticalTiling", settings.verticalTiling);
            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    GlitchRenderPass pass;

    public override void Create()
    {
        pass = new GlitchRenderPass("Glitch");
        name = "Glitch";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
