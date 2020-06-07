Shader "SnapshotProURP/Painting"
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
			int _KernelSize;

			struct region
			{
				float3 mean;
				float variance;
			};

			region calcRegion(int2 lower, int2 upper, int samples, float2 uv)
			{
				region r;
				float3 sum = 0.0;
				float3 squareSum = 0.0;

				for (int x = lower.x; x <= upper.x; ++x)
				{
					for (int y = lower.y; y <= upper.y; ++y)
					{
						float2 offset = float2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
						float3 tex = tex2D(_MainTex, uv + offset);
						sum += tex;
						squareSum += tex * tex;
					}
				}

				r.mean = sum / samples;
				float3 variance = abs((squareSum / samples) - (r.mean * r.mean));
				r.variance = length(variance);

				return r;
			}

            float4 frag (v2f_img i) : SV_Target
            {
				int upper = (_KernelSize - 1) / 2;
				int lower = -upper;

				int samples = (upper + 1) * (upper + 1);

				region regionA = calcRegion(int2(lower, lower), int2(0, 0), samples, i.uv);
				region regionB = calcRegion(int2(0, lower), int2(upper, 0), samples, i.uv);
				region regionC = calcRegion(int2(lower, 0), int2(0, upper), samples, i.uv);
				region regionD = calcRegion(int2(0, 0), int2(upper, upper), samples, i.uv);

				float3 col = regionA.mean;
				float minVar = regionA.variance;

				float testVal;

				testVal = step(regionB.variance, minVar);
				col = lerp(col, regionB.mean, testVal);
				minVar = lerp(minVar, regionB.variance, testVal);

				testVal = step(regionC.variance, minVar);
				col = lerp(col, regionC.mean, testVal);
				minVar = lerp(minVar, regionC.variance, testVal);

				testVal = step(regionD.variance, minVar);
				col = lerp(col, regionD.mean, testVal);

				return float4(col, 1.0);
            }
            ENDCG
        }
    }
}
