using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Underwater : ScriptableRendererFeature
{
    [System.Serializable]
    public class UnderwaterSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Tooltip("Displacement texture for surface waves.")]
        public Texture2D bumpMap = null;

        [Range(0.0f, 10.0f), Tooltip("Strength/size of the waves.")]
        public float strength = 3.0f;

        [Tooltip("Tint of the underwater fog.")]
        public Color waterColor = Color.cyan;

        [Range(0.0f, 1.0f), Tooltip("Strength of the underwater fog.")]
        public float fogStrength = 0.1f;
    }

    public UnderwaterSettings settings = new UnderwaterSettings();

    class UnderwaterRenderPass : ScriptableRenderPass
    {
        private Material material;

        public UnderwaterSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Underwater"));
        }

        public UnderwaterRenderPass(string profilerTag)
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

            cmd.SetGlobalTexture("_BumpMap", settings.bumpMap);
            cmd.SetGlobalFloat("_Strength", settings.strength);
            cmd.SetGlobalColor("_WaterColor", settings.waterColor);
            cmd.SetGlobalFloat("_FogStrength", settings.fogStrength);
            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    UnderwaterRenderPass pass;

    public override void Create()
    {
        pass = new UnderwaterRenderPass("Underwater");
        name = "Underwater";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
