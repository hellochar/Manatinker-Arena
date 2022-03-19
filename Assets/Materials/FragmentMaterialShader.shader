Shader "Unlit/FragmentMaterial"
{
    Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

        [PerRendererData] _Inflow ("Inflow", Float) = 0.5
        [PerRendererData] _ManaPercentage ("Mana Percentage", Float) = 0.5
        [PerRendererData] _Intensity ("Intensity", Float) = 1
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
			float _Intensity;

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
				fixed4 s = SampleSpriteTexture (IN.texcoord);
				float brightness = (0.299*s.r + 0.587*s.g + 0.114*s.b);
				float brightnessDiff = brightness - 0.5;

				fixed4 output = s * 1.50f + fixed4(0.01, 0.02, 0.03, 0);

				output *= 1.0 + brightnessDiff * 0.5;
				output *= _Intensity;


				if (IN.texcoord.x < _ManaPercentage) {
					// draw bright filled in here
					// float intensityMax = lerp(1, 2, smoothstep(0, 1, _Inflow));
					float intensityMax = 2; //  lerp(1, 2, smoothstep(0, 1, _Inflow));
					float divider = lerp(0.03, 0.1, smoothstep(0, 1, _Inflow));
					// float intensityMax = 2;
					float xDist = (_ManaPercentage - IN.texcoord.x) / divider;
					float nearEdgeScalar = clamp(1 / xDist, 0.2, intensityMax);
					if (_ManaPercentage > 0.999) {
						nearEdgeScalar += 0.33;
					}
					output.rgb += IN.color * nearEdgeScalar;
				}
				return output;
			}
		ENDCG
		}
	}
}
