using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Mosaic : ScriptableRendererFeature
{
    [System.Serializable]
    public class MosaicSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Tooltip("Texture to overlay onto each mosaic tile.")]
        public Texture2D overlayTexture;

        [Tooltip("Colour of texture overlay.")]
        public Color overlayColor = Color.white;

        [Range(5, 500), Tooltip("Number of tiles on the x-axis.")]
        public int xTileCount = 100;

        [Tooltip("Use sharper point filtering when downsampling?")]
        public bool usePointFiltering = true;
    }

    public MosaicSettings settings = new MosaicSettings();

    class MosaicRenderPass : ScriptableRenderPass
    {
        private Material material;

        public MosaicSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Mosaic"));
        }

        public MosaicRenderPass(string profilerTag)
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

            int yTileCount = Mathf.RoundToInt((float)Screen.height / Screen.width * settings.xTileCount);

            int mosaicID = Shader.PropertyToID("BlurRT");
            FilterMode filterMode = settings.usePointFiltering ? FilterMode.Point : FilterMode.Bilinear;
            cmd.GetTemporaryRT(mosaicID, settings.xTileCount, yTileCount, 0, filterMode, RenderTextureFormat.ARGB32);

            RenderTargetIdentifier mosaicRT = new RenderTargetIdentifier(mosaicID);

            cmd.SetGlobalTexture("_OverlayTex", settings.overlayTexture ?? Texture2D.whiteTexture);
            cmd.SetGlobalColor("_OverlayColor", settings.overlayColor);
            cmd.SetGlobalInt("_XTileCount", settings.xTileCount);
            cmd.SetGlobalInt("_YTileCount", yTileCount);

            cmd.Blit(source, mosaicRT);
            cmd.Blit(mosaicRT, source, material);

            cmd.ReleaseTemporaryRT(mosaicID);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    MosaicRenderPass pass;

    public override void Create()
    {
        pass = new MosaicRenderPass("Mosaic");
        name = "Mosaic";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
