using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

namespace TJ.PostProcessing.DepthNormal
{
    public class DepthNormalsPass : CustomPostProcessingPass<DepthNormals>
    {
        protected override string RenderTag => "DepthNormals";

        private static readonly int weightID = Shader.PropertyToID("_Weight");
        private static readonly int depthOrNormalID = Shader.PropertyToID("_DepthOrNormal");
        private static readonly int viewToWorldID = Shader.PropertyToID("_ViewToWorld");

        protected override void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
        }

        public DepthNormalsPass(RenderPassEvent renderPassEvent, Shader shader)
            : base(renderPassEvent, shader)
        { 
        }

        protected override void Render(CommandBuffer commandBuffer, ref RenderingData renderingData, RenderTargetIdentifier source, RenderTargetIdentifier dest)
        {
            var viewToWorldMatrix = renderingData.cameraData.camera.cameraToWorldMatrix;

            Material.SetFloat(weightID, Component.weight.value);
            Material.SetFloat(depthOrNormalID, Component.depthOrNormal.value);
            Material.SetMatrix(viewToWorldID, viewToWorldMatrix);

            base.Render(commandBuffer, ref renderingData, source, dest);
        }

        public override void Setup(in RenderTargetIdentifier renderTargetIdentifier)
        {
            base.Setup(renderTargetIdentifier);
            ConfigureInput(ScriptableRenderPassInput.Normal);
        }

        protected override bool IsActive()
            => Component.weight.value > 0 &&
               Component.IsActive();
    }
}


