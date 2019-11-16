Shader "UI/Sphere"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
		_OutterRadius("Outter Radius", Range(0, 1)) = 1
		_InnerRadius("Inner Radius", Range(0, 1)) = 0
		_StartAngle("Start Angle", Range(0, 360)) = 0
		_EndAngle("End Angle", Range(0, 360)) = 360
		_NumStride("Stride", Int) = 0
		_StrideWidth("Stride Width", Float) = 5
		_Value("Value", Range(0, 1)) = 1
    }


    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0

            #include "UnityCG.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            sampler2D _MainTex;
			float _OutterRadius;
			float _InnerRadius;
			float _StartAngle;
			float _EndAngle;
			int _NumStride;
			float _StrideWidth;
			float _Value;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 d = 2 * (float2(0.5, 0.5) - i.uv);

				if (length(d) < _InnerRadius)
					discard;

				if (length(d) > _OutterRadius)
					discard;


				float2 direction = normalize(d);

				float s = -sign(direction.y);
				float angle = (1 - s) * 180 + (s * acos(dot(float2(-1, 0), direction))) * 180 / 3.1415926;

				float pivotAngle = lerp(_StartAngle, _EndAngle, _Value);

				if (angle > pivotAngle)
					discard;

				if (angle < _StartAngle)
					discard;


				int n = _NumStride + 1;
				int a = (_EndAngle - _StartAngle) / n;
				//for (int i = 0; i < n; i++)
				//{
				//	int ai = i * a;
				//	int da = angle - _StartAngle;

				//	if (da > ai - _StrideWidth / 2)
				//		discard;

				//	if (da < ai + _StrideWidth / 2)
				//		discard;
				//}


				half4 o = half4(0, 0, 0, 1);

                o.rgb = tex2D(_MainTex, i.uv);

                return o;
            }
            ENDCG
        }
    }
}
