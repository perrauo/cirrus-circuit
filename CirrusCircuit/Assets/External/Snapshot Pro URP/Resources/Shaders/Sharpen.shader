Shader "SnapshotProURP/Sharpen"
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
			float _Intensity;

            float4 frag (v2f_img i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;
				col += 4.0f * col * _Intensity;

				float2 s = _MainTex_TexelSize;
				col -= tex2D(_MainTex, i.uv + float2(0,	   -s.y)).xyz * _Intensity;
				col -= tex2D(_MainTex, i.uv + float2(-s.x,    0)).xyz * _Intensity;
				col -= tex2D(_MainTex, i.uv + float2(s.x,     0)).xyz * _Intensity;
				col -= tex2D(_MainTex, i.uv + float2(0,     s.y)).xyz * _Intensity;

				return float4(col, 1.0f);
            }
            ENDCG
        }
    }
}
