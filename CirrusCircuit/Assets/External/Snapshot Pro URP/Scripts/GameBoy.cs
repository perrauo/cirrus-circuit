using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameBoy : ScriptableRendererFeature
{
    [System.Serializable]
    public class GameBoySettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Tooltip("Darkest colour.")]
        public Color darkest = new Color(0.11f, 0.21f, 0.08f);

        [Tooltip("Second darkest colour.")]
        public Color dark = new Color(0.24f, 0.38f, 0.21f);

        [Tooltip("Second lightest colour.")]
        public Color light = new Color(0.57f, 0.67f, 0.21f);

        [Tooltip("Lightest colour.")]
        public Color lightest = new Color(0.75f, 0.82f, 0.46f);
    }

    public GameBoySettings settings = new GameBoySettings();

    class GameBoyRenderPass : ScriptableRenderPass
    {
        private Material material;

        public GameBoySettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/GameBoy"));
        }

        public GameBoyRenderPass(string profilerTag)
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

            cmd.SetGlobalColor("_GBDarkest", settings.darkest);
            cmd.SetGlobalColor("_GBDark", settings.dark);
            cmd.SetGlobalColor("_GBLight", settings.light);
            cmd.SetGlobalColor("_GBLightest", settings.lightest);
            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    GameBoyRenderPass pass;

    public override void Create()
    {
        pass = new GameBoyRenderPass("GameBoy");
        name = "Game Boy";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
