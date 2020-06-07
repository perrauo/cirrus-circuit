using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Drawing : ScriptableRendererFeature
{
    [System.Serializable]
    public class DrawingSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Tooltip("Drawing overlay texture.")]
        public Texture2D drawingTex;

        [Range(0.0f, 5.0f), Tooltip("Time taken (in seconds) per animation cycle. Set to zero for no animation.")]
        public float animCycleTime = 0.75f;

        [Range(0.0f, 1.0f), Tooltip("Strength of the effect.")]
        public float strength = 0.5f;

        [Range(1.0f, 50.0f), Tooltip("Number of times the drawing texture is tiled.")]
        public float tiling = 25.0f;

        [Range(0.0f, 5.0f), Tooltip("Amount of UV smudging based on drawing texture colour values.")]
        public float smudge = 0.001f;

        [Range(0.0f, 1.01f), Tooltip("Pixels past this depth threshold will not be 'drawn on'.")]
        public float depthThreshold = 0.99f;
    }

    public DrawingSettings settings = new DrawingSettings();

    class DrawingRenderPass : ScriptableRenderPass
    {
        private Material material;

        public DrawingSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Drawing"));
        }

        public DrawingRenderPass(string profilerTag)
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

            bool isOffset = false;

            if(settings.animCycleTime > 0.0f)
            {
                isOffset = (Time.time % settings.animCycleTime) < (settings.animCycleTime / 2.0f);
            }

            cmd.SetGlobalTexture("_DrawingTex", settings.drawingTex ?? Texture2D.whiteTexture);

            cmd.SetGlobalFloat("_OverlayOffset", isOffset ? 0.5f : 0.0f);
            cmd.SetGlobalFloat("_Strength", settings.strength);
            cmd.SetGlobalFloat("_Tiling", settings.tiling);
            cmd.SetGlobalFloat("_Smudge", settings.smudge);
            cmd.SetGlobalFloat("_DepthThreshold", settings.depthThreshold);

            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    DrawingRenderPass pass;

    public override void Create()
    {
        pass = new DrawingRenderPass("Drawing");
        name = "Drawing";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
