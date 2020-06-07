using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Cutout : ScriptableRendererFeature
{
    [System.Serializable]
    public class CutoutSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Tooltip("The texture to use for the cutout.")]
        public Texture2D cutoutTexture = null;

        [Tooltip("The colour of the area outside the cutout.")]
        public Color borderColor = Color.white;

        [Tooltip("Should the cutout texture stretch to fit the screen's aspect ratio?")]
        public bool stretch = false;
    }

    public CutoutSettings settings = new CutoutSettings();

    class CutoutRenderPass : ScriptableRenderPass
    {
        private Material material;

        public CutoutSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Cutout"));
        }

        public CutoutRenderPass(string profilerTag)
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

            cmd.SetGlobalTexture("_CutoutTex", settings.cutoutTexture);
            cmd.SetGlobalColor("_BorderColor", settings.borderColor);
            cmd.SetGlobalInt("_Stretch", settings.stretch ? 1 : 0);
            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    CutoutRenderPass pass;

    public override void Create()
    {
        pass = new CutoutRenderPass("Cutout");
        name = "Cutout";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
