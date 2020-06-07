Shader "SnapshotProURP/BasicDither"
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
			sampler2D _NoiseTex;
			float4 _NoiseTex_TexelSize;
			float _NoiseSize;

			float4 _LightColor;
			float4 _DarkColor;

            float4 frag (v2f_img i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;
				float lum = dot(col, float3(0.3f, 0.59f, 0.11f));

				float2 noiseUV = i.uv * _NoiseTex_TexelSize.xy * _ScreenParams.xy / _NoiseSize;
				float3 noiseColor = tex2D(_NoiseTex, noiseUV);
				float threshold = dot(noiseColor, float3(0.3f, 0.59f, 0.11f));

				col = lum < threshold ? _DarkColor : _LightColor;

				return float4(col, 1.0f);
            }
            ENDCG
        }
    }
}
