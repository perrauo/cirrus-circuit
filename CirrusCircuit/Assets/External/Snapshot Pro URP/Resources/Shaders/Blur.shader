Shader "SnapshotProURP/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

		CGINCLUDE

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		uint _KernelSize;
		float _Spread;

		static const float E = 2.71828f;
		static const float TWO_PI = 6.28253f;

		float gaussian(int x)
		{
			float sigmaSqu = _Spread * _Spread;
			return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(x * x) / (2 * sigmaSqu));
		}

		ENDCG

        Pass
        {
			Name "Horizontal"

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_horizontal

            #include "UnityCG.cginc"

            float4 frag_horizontal (v2f_img i) : SV_Target
			{
				float3 col = float3(0.0f, 0.0f, 0.0f);
				float kernelSum = 0.0f;

				int upper = ((_KernelSize - 1) / 2);
				int lower = -upper;

				float2 uv;

				for (int x = lower; x <= upper; ++x)
				{
					float gauss = gaussian(x);
					kernelSum += gauss;
					uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);
					col += gauss * tex2D(_MainTex, uv).xyz;
				}

				col /= kernelSum;

				return float4(col, 1.0f);
			}
            ENDCG
        }

		Pass
        {
			Name "Vertical"

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_vertical

            #include "UnityCG.cginc"

            float4 frag_vertical (v2f_img i) : SV_Target
			{
				float3 col = float3(0.0f, 0.0f, 0.0f);
				float kernelSum = 0.0f;

				int upper = ((_KernelSize - 1) / 2);
				int lower = -upper;

				float2 uv;

				for (int y = lower; y <= upper; ++y)
				{
					float gauss = gaussian(y);
					kernelSum += gauss;
					uv = i.uv + float2(0.0f, _MainTex_TexelSize.y * y);
					col += gauss * tex2D(_MainTex, uv).xyz;
				}

				col /= kernelSum;
				return float4(col, 1.0f);
			}

            ENDCG
        }
    }
}
