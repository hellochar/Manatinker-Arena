Shader "Unlit/ManaCoverShader"
{
    Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

        [PerRendererData] _Inflow ("Inflow", Float) = 0.5
        [PerRendererData] _ManaPercentage ("Mana Percentage", Float) = 0.5
		_MissingColor ("Missing Color", Color) = (0.2, 0.21, 0.23, 0)
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

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

            float _Inflow;
            float _ManaPercentage;
			fixed4 _MissingColor;

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
				fixed4 texColor = SampleSpriteTexture (IN.texcoord);
				fixed4 tint = IN.color;

				// this is just the normal sprite color
                // fixed4 output = texColor * tint;
                fixed4 output = tint * texColor.a;

				if (_ManaPercentage > 0.999) {
					return fixed4(0, 0, 0, 0);
				}

				if (IN.texcoord.x < _ManaPercentage) {
					// draw bright filled in here
					float intensity = lerp(0.5f, 2, smoothstep(0, 1, _Inflow * _Inflow));
					output.rgb = output.rgb * intensity;
				} else {
					return fixed4(0, 0, 0, 0);
					// output *= _MissingColor;
					// output.rgb = lerp(output.rgb, _MissingColor.rgb, intensity);
					// output.a = lerp(output.a, _MissingColor.a, intensity);
				}
				return output;
			}
		ENDCG
		}
	}
}
