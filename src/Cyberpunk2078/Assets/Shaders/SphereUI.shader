Shader "UI/Sphere"
{
    Properties
    {
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1, 1, 1, 1)

		//_StencilComp("Stencil Comparison", Float) = 8
		//_Stencil("Stencil ID", Float) = 0
		//_StencilOp("Stencil Operation", Float) = 0
		//_StencilWriteMask("Stencil Write Mask", Float) = 255
		//_StencilReadMask("Stencil Read Mask", Float) = 255

		//_ColorMask("Color Mask", Float) = 15

		_OutterRadius("Outter Radius", Range(0, 1)) = 1
		_InnerRadius("Inner Radius", Range(0, 1)) = 0
		_StartAngle("Start Angle", Range(0, 360)) = 0
		_EndAngle("End Angle", Range(0, 360)) = 360
		_NumStride("Stride", Int) = 1
		_StrideWidth("Stride Width", Float) = 5
    }


    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		/*Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}*/

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		//ColorMask[_ColorMask]


		Pass
		{
			Name "Sphere"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"


			struct appdata_t
			{
				float4 position : POSITION;
				float4 color    : COLOR;
				float2 uv		: TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 position			: SV_POSITION;
				fixed4 color			: COLOR;
				float2 uv				: TEXCOORD0;
				float4 worldPosition	: TEXCOORD1;

				UNITY_VERTEX_OUTPUT_STEREO
			};


			sampler2D _MainTex;
			fixed4 _Color;
			float4 _MainTex_ST;

			float _OutterRadius;
			float _InnerRadius;
			float _StartAngle;
			float _EndAngle;
			int _NumStride;
			float _StrideWidth;


			v2f vert(appdata_t v)
			{
				v2f OUT;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.position = UnityObjectToClipPos(v.position);
				OUT.color = v.color * _Color;
				OUT.uv = TRANSFORM_TEX(v.uv, _MainTex);
				OUT.worldPosition = v.position;


				return OUT;
			}



			fixed4 frag(v2f IN) : SV_Target
			{
				float2 d = 2 * (float2(0.5, 0.5) - IN.uv);

				if (length(d) < _InnerRadius)
					discard;

				if (length(d) > _OutterRadius)
					discard;


				float2 direction = normalize(d);

				float s = -sign(direction.y);
				float angle = (1 - s) * 180 + (s * acos(dot(float2(-1, 0), direction))) * 180 / 3.1415926;

				if (angle > _EndAngle)
					discard;

				if (angle < _StartAngle)
					discard;


				float a = (_EndAngle - _StartAngle - _StrideWidth * (_NumStride - 1)) / _NumStride;

				if (angle % (a + _StrideWidth) > a)
					discard;


				half4 color = tex2D(_MainTex, IN.uv) * IN.color * _Color;


				return color;
			}

		ENDCG
		}
    }
}
