Shader "SnapshotProURP/LightStreaks"
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
			Name "HorizontalBlur"

			CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_horizontal

            #include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			uint _KernelSize;
			float _Spread;

			float _LuminanceThreshold;

			static const float E = 2.71828f;
			static const float TWO_PI = 6.28253f;

			float gaussian(int x)
			{
				float sigmaSqu = _Spread * _Spread;
				return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(x * x) / (2 * sigmaSqu));
			}

			float4 frag_horizontal(v2f_img i) : SV_Target
			{
				float3 light = 0.0f;
				float kernelSum = 0.0f;

				int upper = ((_KernelSize - 1) / 2);
				int lower = -upper;

				float2 uv;

				for (int x = lower; x <= upper; ++x)
				{
					float gauss = gaussian(x);
					kernelSum += gauss;
					uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);

					float3 newLight = tex2D(_MainTex, uv).xyz;
					float lum = dot(newLight, float3(0.3f, 0.59f, 0.11f));
					light += step(_LuminanceThreshold, lum) * newLight * gauss;
				}

				light /= kernelSum;

				return float4(light, 1.0f);
			}
            ENDCG
        }

		Pass
		{
			Name "Overlay"

			CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _BlurTex;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 mainCol = tex2D(_MainTex, i.uv);
				float4 blurCol = tex2D(_BlurTex, i.uv);

				return mainCol + blurCol;
			}
            ENDCG
		}
    }
}
