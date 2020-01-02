Shader "UI/GradientAlpha"
{
    Properties
    {
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1, 1, 1, 1)

		_MaxAlpha("Max Alpha", Range(0, 1)) = 1
		_MinAlpha("Min Alpha", Range(0, 1)) = 0
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

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha


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

			float _MaxAlpha;
			float _MinAlpha;


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
				half4 color = tex2D(_MainTex, IN.uv) * IN.color * _Color;

				color.a = lerp(_MinAlpha, _MaxAlpha, 1 - IN.uv.y);


				return color;
			}

		ENDCG
		}
    }
}
