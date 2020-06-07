using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Silhouette : ScriptableRendererFeature
{
    [System.Serializable]
    public class SilhouetteSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Tooltip("Color at the camera's near clip plane.")]
        public Color nearColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        [Tooltip("Color at the camera's far clip plane.")]
        public Color farColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public SilhouetteSettings settings = new SilhouetteSettings();

    class SilhouetteRenderPass : ScriptableRenderPass
    {
        private Material material;

        public SilhouetteSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Silhouette"));
        }

        public SilhouetteRenderPass(string profilerTag)
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
            
            cmd.SetGlobalColor("_NearColor", settings.nearColor);
            cmd.SetGlobalColor("_FarColor", settings.farColor);
            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    SilhouetteRenderPass pass;

    public override void Create()
    {
        pass = new SilhouetteRenderPass("Silhouette");
        name = "Silhouette";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
