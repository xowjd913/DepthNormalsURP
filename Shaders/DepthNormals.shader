Shader "Custom/PostProcessing/DepthNormals"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Weight("Weight", float) = 1
		_DepthOrNormal("Depth or normal", Range(0.0, 1.0)) = 0.5
	}
	SubShader
	{
		Tags
		{
			"RenderPipeline"="UniversalPipeline"
		}
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			CBUFFER_START(UnityPerMaterial)

			TEXTURE2D(_MainTex);		SAMPLER(sampler_MainTex);
			float4 _MainTex_ST;
			float _Weight;
			float _DepthOrNormal;
			float4x4 _ViewToWorld;

			CBUFFER_END

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
            float4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

            float depth = SampleSceneDepth(i.uv);
            float3 normal = SampleSceneNormals(i.uv);

            depth = Linear01Depth(depth, _ZBufferParams);
            normal = mul((float3x3)_ViewToWorld, normal);

            half4 color = float4(lerp(float3(depth,depth,depth), normal, _DepthOrNormal),1);

            return lerp(mainColor, color, _Weight);
			}

			ENDHLSL
		}
	}
}
