//Enhanced version, including "sprite outline size" and allowing you to switch between inner or outer
//be careful about increasing the outline size, since you might have a slower performance (sampling a texture 4*size times more)

Shader "Unlit/MasterSpriteOutline HLSL"
{
    Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		[MaterialToggle] Inner("Inner Outline", Int) = 0
		_OutlineSize("Outline Size", Int) = 1

    }
    SubShader
    {
		//Tags
		//{
		//	"RenderType" = "Transparent"
		//}

		Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile _ INNER_ON

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

			fixed4 _OutlineColor;
			int _OutlineSize;
            int _ShadowSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

				fixed outline;
				for (int j = 1; j < _OutlineSize + 1; j++) {
					fixed leftPixel = tex2D(_MainTex, i.uv + float2(j* -_MainTex_TexelSize.x, 0)).a;
					fixed upPixel = tex2D(_MainTex, i.uv + float2(0, j * _MainTex_TexelSize.y)).a;
					fixed rightPixel = tex2D(_MainTex, i.uv + float2(j * _MainTex_TexelSize.x, 0)).a;
					fixed bottomPixel = tex2D(_MainTex, i.uv + float2(0, j * -_MainTex_TexelSize.y)).a;


#ifdef INNER_ON
					outline = max(outline,(1 - leftPixel * upPixel * rightPixel * bottomPixel) * col.a);
#else
					outline = max(max(max(leftPixel, upPixel), max(rightPixel, bottomPixel)) - col.a, outline);
#endif
				}

                for (int j = 1; j >= _OutlineSize && j < _ShadowSize; j++)
                {
                    fixed leftPixel = tex2D(_MainTex, i.uv + float2(j * -_MainTex_TexelSize.x, 0)).a;
                    fixed upPixel = tex2D(_MainTex, i.uv + float2(0, j * _MainTex_TexelSize.y)).a;
                    fixed rightPixel = tex2D(_MainTex, i.uv + float2(j * _MainTex_TexelSize.x, 0)).a;
                    fixed bottomPixel = tex2D(_MainTex, i.uv + float2(0, j * -_MainTex_TexelSize.y)).a;


#ifdef INNER_ON
                    outline = max(outline, (1 - leftPixel * upPixel * rightPixel * bottomPixel) * col.a);
#else
                    outline = max(max(max(leftPixel, upPixel), max(rightPixel, bottomPixel)) - col.a, outline);
#endif
                }

                return lerp(col, _OutlineColor, outline);
            }
            ENDCG
        }
    }
}
