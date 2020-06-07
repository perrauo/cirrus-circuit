Shader "SnapshotProURP/SobelOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
			CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

			sampler2D _MainTex;
			float2 _MainTex_TexelSize;

			float _Threshold;
			float4 _OutlineColor;
			float4 _BackgroundColor;

			float sobel(float2 uv)
			{
				float3 x = 0;
				float3 y = 0;

				float2 pixel = _MainTex_TexelSize;

				x += tex2D(_MainTex, uv + float2(-pixel.x, -pixel.y)).xyz * -1.0f;
				x += tex2D(_MainTex, uv + float2(-pixel.x, 0)).xyz * -2.0f;
				x += tex2D(_MainTex, uv + float2(-pixel.x, pixel.y)).xyz * -1.0f;

				x += tex2D(_MainTex, uv + float2(pixel.x, -pixel.y)).xyz * 1.0f;
				x += tex2D(_MainTex, uv + float2(pixel.x, 0)).xyz * 2.0f;
				x += tex2D(_MainTex, uv + float2(pixel.x, pixel.y)).xyz * 1.0f;

				y += tex2D(_MainTex, uv + float2(-pixel.x, -pixel.y)).xyz * -1.0f;
				y += tex2D(_MainTex, uv + float2(0, -pixel.y)).xyz * -2.0f;
				y += tex2D(_MainTex, uv + float2(pixel.x, -pixel.y)).xyz * -1.0f;

				y += tex2D(_MainTex, uv + float2(-pixel.x, pixel.y)).xyz * 1.0f;
				y += tex2D(_MainTex, uv + float2(0, pixel.y)).xyz * 2.0f;
				y += tex2D(_MainTex, uv + float2(pixel.x, pixel.y)).xyz * 1.0f;

				float xLum = dot(x, float3(0.2126729, 0.7151522, 0.0721750));
				float yLum = dot(y, float3(0.2126729, 0.7151522, 0.0721750));

				return sqrt(xLum * xLum + yLum * yLum);
			}

            float4 frag (v2f_img i) : SV_Target
            {
				float s = sobel(i.uv);
				float4 sobelCol = s > _Threshold ? _OutlineColor : _BackgroundColor;
				float4 col = tex2D(_MainTex, i.uv);

				return lerp(col, sobelCol, sobelCol.a);
            }
            ENDCG
        }
    }
}
