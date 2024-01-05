using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TJ.PostProcessing.DepthNormal
{

    public class DepthNormals : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter isActive = new BoolParameter(false);
        public ClampedFloatParameter weight = new ClampedFloatParameter(0.5f, 0f, 1f);
        public ClampedFloatParameter depthOrNormal = new ClampedFloatParameter(0.5f, 0f, 1f);

        public bool IsActive() => weight.value > 0f && isActive.value;

        public bool IsTileCompatible() => false;
    }

}
