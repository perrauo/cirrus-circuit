using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FancyOutline : ScriptableRendererFeature
{
    [System.Serializable]
    public class FancyOutlineSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Tooltip("Color of the outlines.")]
        public Color outlineColor = Color.black;

        [Range(0.0f, 1.0f), Tooltip("Threshold for colour-based edge detection.")]
        public float colorSensitivity = 0.1f;

        [Range(0.0f, 1.0f), Tooltip("Strength of colour-based edges.")]
        public float colorStrength = 0.5f;

        [Range(0.0f, 1.0f), Tooltip("Threshold for depth-based edge detection.")]
        public float depthSensitivity = 0.01f;

        [Range(0.0f, 1.0f), Tooltip("Strength of depth-based edges.")]
        public float depthStrength = 0.75f;

        [Range(0.0f, 1.0f), Tooltip("Threshold for normal-based edge detection.")]
        public float normalSensitivity = 0.1f;

        [Range(0.0f, 1.0f), Tooltip("Strength of normal-based edges.")]
        public float normalStrength = 0.75f;

        [Tooltip("Pixels past this depth threshold will not be edge-detected.")]
        public float depthThreshold = 0.99f;
    }

    public FancyOutlineSettings settings = new FancyOutlineSettings();

    class FancyOutlineRenderPass : ScriptableRenderPass
    {
        private Material material;

        public FancyOutlineSettings settings;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("SnapshotProURP/Outline"));
        }

        public FancyOutlineRenderPass(string profilerTag)
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

            cmd.SetGlobalColor("_OutlineColor", settings.outlineColor);
            cmd.SetGlobalFloat("_ColorSensitivity", settings.colorSensitivity);
            cmd.SetGlobalFloat("_ColorStrength", settings.colorStrength);
            cmd.SetGlobalFloat("_DepthSensitivity", settings.depthSensitivity);
            cmd.SetGlobalFloat("_DepthStrength", settings.depthStrength);
            cmd.SetGlobalFloat("_NormalsSensitivity", settings.normalSensitivity);
            cmd.SetGlobalFloat("_NormalsStrength", settings.normalStrength);
            cmd.SetGlobalFloat("_DepthThreshold", settings.depthThreshold);

            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    DepthNormalsPass depthNormalsPass;
    FancyOutlineRenderPass pass;

    public override void Create()
    {
        depthNormalsPass = new DepthNormalsPass();

        pass = new FancyOutlineRenderPass("FancyOutline");
        name = "Fancy Outline";

        pass.settings = settings;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        depthNormalsPass.Setup(renderingData.cameraData.cameraTargetDescriptor);
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(depthNormalsPass);
        renderer.EnqueuePass(pass);
    }
}
