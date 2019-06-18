// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/Default Font - Extra Texture" {
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_OverlayTex ("Overlay Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(USE_YPCBCR)] _UseYpCbCr("Use YpCbCr", Float) = 0
		_ChromaTex("Chroma", 2D) = "gray" {}
	}

	SubShader {

		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		
		Lighting Off 
		Cull Off 
		ZTest [unity_GUIZTestMode]
		ZWrite Off 
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "../../Resources/Shaders/AVProVideo.cginc"

			#pragma multi_compile __ USE_YPCBCR

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _OverlayTex;
#if USE_YPCBCR
			sampler2D _ChromaTex;
#endif
			uniform float4 _MainTex_ST;
			uniform float4 _OverlayTex_ST;
			uniform fixed4 _Color;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color * _Color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
#ifdef UNITY_HALF_TEXEL_OFFSET
				o.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				o.texcoord2 = (o.vertex.xy + 1) / 2;
				o.texcoord2.y = 1 - o.texcoord2.y;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color;
#if USE_YPCBCR
	#if SHADER_API_METAL || SHADER_API_GLES || SHADER_API_GLES3
				float3 ypcbcr = float3(tex2D(_OverlayTex, i.texcoord2).r, tex2D(_ChromaTex, i.texcoord2).rg);
	#else
				float3 ypcbcr = float3(tex2D(_OverlayTex, i.texcoord2).r, tex2D(_ChromaTex, i.texcoord2).ra);
	#endif
				fixed4 overlay = fixed4(Convert420YpCbCr8ToRGB(ypcbcr), 1.0);
#else
				fixed4 overlay = fixed4(tex2D(_OverlayTex, i.texcoord2).rgb, 1.0);
#endif
				col.a *= tex2D(_MainTex, i.texcoord).a;
				clip (col.a - 0.01);
				col *= overlay;
				return col;
			}
			ENDCG 
		}
	}
}
