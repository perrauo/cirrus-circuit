using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class DepthNormalsPass : ScriptableRenderPass
{
    private Material material;
    private FilteringSettings filteringSettings;

    private RenderTextureDescriptor descriptor;
    private RenderTargetHandle depthNormalsHandle;

    private string profilerTag = "DepthNormalsPass";

    public void Setup(RenderTextureDescriptor descriptor)
    {
        this.descriptor = descriptor;
        descriptor.colorFormat = RenderTextureFormat.ARGB32;
        descriptor.depthBufferBits = 32;

        depthNormalsHandle.Init("_CameraDepthNormalsTexture");

        filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        material = new Material(Shader.Find("Hidden/Internal-DepthNormalsTexture"));
        renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        cmd.GetTemporaryRT(depthNormalsHandle.id, descriptor, FilterMode.Point);
        ConfigureTarget(depthNormalsHandle.Identifier());
        ConfigureClear(ClearFlag.All, Color.black);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

        var shaderTag = new ShaderTagId("DepthOnly");
        var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
        var drawSettings = CreateDrawingSettings(shaderTag, ref renderingData, sortFlags);
        drawSettings.perObjectData = PerObjectData.None;

        if (renderingData.cameraData.isStereoEnabled)
        {
            context.StartMultiEye(renderingData.cameraData.camera);
        }

        drawSettings.overrideMaterial = material;

        context.DrawRenderers(renderingData.cullResults, ref drawSettings,
                ref filteringSettings);

        cmd.SetGlobalTexture("_CameraDepthNormalsTexture", depthNormalsHandle.id);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        if (depthNormalsHandle != RenderTargetHandle.CameraTarget)
        {
            cmd.ReleaseTemporaryRT(depthNormalsHandle.id);
            depthNormalsHandle = RenderTargetHandle.CameraTarget;
        }
    }
}
