namespace TJ.PostProcessing
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public abstract class CustomPostProcessingPass<T> : ScriptableRenderPass where T : VolumeComponent
    {
        protected static readonly int TempColorBufferId = UnityEngine.Shader.PropertyToID("_TempColorBuffer");

        protected Shader Shader;
        protected Material Material;
        protected T Component;

        private RenderTargetIdentifier renderTargetIdentifier;
        private RenderTargetIdentifier tempRenderTargetIdentifier;
        protected abstract string RenderTag { get; }

        public CustomPostProcessingPass(RenderPassEvent renderPassEvent, Shader shader)
        {
            this.renderPassEvent = renderPassEvent;
            Shader = shader;

            if (shader == null)
            {
                return;
            }

            Material = CoreUtils.CreateEngineMaterial(shader);
        }

        public virtual void Setup(in RenderTargetIdentifier renderTargetIdentifier)
        {
            this.renderTargetIdentifier = renderTargetIdentifier;

            tempRenderTargetIdentifier = new RenderTargetIdentifier(TempColorBufferId);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Material == null || !renderingData.cameraData.postProcessEnabled)
            {
                return;
            }

            var volumeStack = VolumeManager.instance.stack;
            Component = volumeStack.GetComponent<T>();
            if (Component == null || !Component.active || !IsActive())
            {
                return;
            }

            var commandBuffer = CommandBufferPool.Get(RenderTag);
            Render(commandBuffer, ref renderingData);
            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        private void Render(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var source = renderTargetIdentifier;
            var dest = tempRenderTargetIdentifier;

            SetupRenderTexture(commandBuffer, ref renderingData);

            BeforeRender(commandBuffer, ref renderingData);

            CopyToTempBuffer(commandBuffer, ref renderingData, source, dest);

            Render(commandBuffer, ref renderingData, dest, source);

            CleanupRenderTexture(commandBuffer, ref renderingData);
        }

        protected virtual void SetupRenderTexture(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;

            var desc = new RenderTextureDescriptor(cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight);
            desc.colorFormat = cameraData.isHdrEnabled ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

            commandBuffer.GetTemporaryRT(TempColorBufferId, desc);
        }

        protected virtual void CleanupRenderTexture(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            commandBuffer.ReleaseTemporaryRT(TempColorBufferId);
        }

        protected virtual void CopyToTempBuffer(CommandBuffer commandBuffer, ref RenderingData renderingData,
            RenderTargetIdentifier source, RenderTargetIdentifier dest)
        {
            commandBuffer.Blit(source, dest);
        }

        protected virtual void Render(CommandBuffer commandBuffer, ref RenderingData renderingData,
            RenderTargetIdentifier source, RenderTargetIdentifier dest)
        {
            commandBuffer.Blit(source, dest, Material);
        }

        protected abstract void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData);

        protected abstract bool IsActive();
    }
}
