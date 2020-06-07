using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Kaleidoscope : ScriptableRendererFeature
{
    [System.Serializable]
    public class KaleidoscopeSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Range(0.0f, 20.0f), Tooltip("The number of radial segments.")]
        public float segmentCount = 6.0f;
    }

    public KaleidoscopeSettings settings = new KaleidoscopeSettings();

    class KaleidoscopeRenderPass : ScriptableRenderPass
    {
        private Material material;

        public KaleidoscopeSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Kaleidoscope"));
        }

        public KaleidoscopeRenderPass(string profilerTag)
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

            cmd.SetGlobalFloat("_SegmentCount", settings.segmentCount);
            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    KaleidoscopeRenderPass pass;

    public override void Create()
    {
        pass = new KaleidoscopeRenderPass("Kaleidoscope");
        name = "Kaleidoscope";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
