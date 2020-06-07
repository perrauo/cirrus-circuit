using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BasicDither : ScriptableRendererFeature
{
    [System.Serializable]
    public class BasicDitherSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Tooltip("Noise texture to use for dither thresholding.")]
        public Texture2D noiseTex = null;

        [Range(0.1f, 100.0f), Tooltip("Size of the noise texture.")]
        public float noiseSize = 1.0f;

        [Tooltip("Color to use for dark sections of the image.")]
        public Color darkColor = Color.black;

        [Tooltip("Color to use for light sections of the image.")]
        public Color lightColor = Color.white;
    }

    public BasicDitherSettings settings = new BasicDitherSettings();

    class BasicDitherRenderPass : ScriptableRenderPass
    {
        private Material material;

        public BasicDitherSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/BasicDither"));
        }

        public BasicDitherRenderPass(string profilerTag)
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

            cmd.SetGlobalTexture("_NoiseTex", settings.noiseTex ?? Texture2D.whiteTexture);
            cmd.SetGlobalFloat("_NoiseSize", settings.noiseSize);
            cmd.SetGlobalColor("_DarkColor", settings.darkColor);
            cmd.SetGlobalColor("_LightColor", settings.lightColor);

            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    BasicDitherRenderPass pass;

    public override void Create()
    {
        pass = new BasicDitherRenderPass("BasicDither");
        name = "Basic Dither";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
