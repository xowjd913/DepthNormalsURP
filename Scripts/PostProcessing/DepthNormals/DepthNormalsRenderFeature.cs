using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TJ.PostProcessing.DepthNormal
{
    using System;
    using UnityEngine.Rendering.Universal;

    public class DepthNormalsRenderFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            public Shader shader;
        }

        public Settings settings = new Settings();

        private DepthNormalsPass depthNormalsPass;

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            depthNormalsPass.Setup(renderer.cameraColorTargetHandle);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(depthNormalsPass);
        }

        public override void Create()
        {
            this.name = "DepthNormals";

            if(settings.shader != null)
                depthNormalsPass = new DepthNormalsPass(settings.renderPassEvent, settings.shader);
        }
    }

}
