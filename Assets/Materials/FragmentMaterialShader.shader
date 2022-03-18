Shader "Unlit/FragmentMaterial"
{
    Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

        [PerRendererData] _Inflow ("Inflow", Float) = 0.5
        [PerRendererData] _ManaPercentage ("Mana Percentage", Float) = 0.5
		_LowFlowScalar ("Low Flow Scalar", Float) = 0.1
		_LowFlowPoint ("Low Flow Point", Float) = 0.1
		_MaxFlowScalar ("Max Flow Scalar", Float) = 0.1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		// Removes transparency completely
		// Blend Off

		// Unity default - premultiplied alpha
		// Blend One OneMinusSrcAlpha

		// entirely use our texture RGB, disregard destination color
		// Blend One Zero
		
		// traditional alpha
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			// Unity parameters
			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

            float _Inflow;
			float _ManaPercentage;

			float _LowFlowScalar;
			float _LowFlowPoint;
			float _MaxFlowScalar;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 texColor = SampleSpriteTexture (IN.texcoord) * IN.color;

				return texColor;

				float colorScalar;
				// if we're near max mana, just do a straight smoothstep without the ramp
				if (_ManaPercentage > 0.999) {
					colorScalar = lerp(1, _MaxFlowScalar, smoothstep(0, 1, _Inflow));
				} else {
					// go down then up
					if (_Inflow < _LowFlowPoint) {
						float downRampT = smoothstep(0, 1, _Inflow / _LowFlowPoint);
						colorScalar = lerp(1, _LowFlowScalar, downRampT * downRampT);
					} else {
						float rampSize = 1 - _LowFlowPoint;
						float upRampT = smoothstep(0, 1, (_Inflow - _LowFlowPoint) / rampSize);
						colorScalar = lerp(_LowFlowScalar, _MaxFlowScalar, upRampT * upRampT);
					}
				}
				texColor.rgb *= colorScalar;
				// if (length(texColor.rgb - fixed3(1, 1, 1)) < 0.5) {
				// 	texColor.rgb = lerp(fixed3(0.5, 0.5, 0.5), texColor.rgb, _Percentage);
				// 	texColor.rgb *= texColor.a;
				// } else {
				// 	texColor.rgb *= texColor.a;
				// 	texColor *= _Color;
				// }
				// texColor.rgb *= 1 + _Percentage * 3;
				return texColor;
			}
		ENDCG
		}
	}
}
