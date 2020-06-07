Shader "SnapshotProURP/GameBoy"
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
			float4 _GBDarkest;
			float4 _GBDark;
			float4 _GBLight;
			float4 _GBLightest;

            float4 frag (v2f_img i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;
				float lum = dot(col, float3(0.3f, 0.59f, 0.11f));
				int gb = lum * 3;

				col = lerp(_GBDarkest, _GBDark, saturate(gb));
				col = lerp(col, _GBLight, saturate(gb - 1.0f));
				col = lerp(col, _GBLightest, saturate(gb - 2.0f));

                return float4(col, 1.0f);
            }
            ENDCG
        }
    }
}
