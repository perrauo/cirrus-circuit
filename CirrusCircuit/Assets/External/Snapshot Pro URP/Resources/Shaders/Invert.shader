Shader "SnapshotProURP/Invert"
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
			float _Blend;

            float4 frag (v2f_img i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;
				col = lerp(col, 1.0f - col, _Blend);
				return float4(col, 1.0f);
            }
            ENDCG
        }
    }
}
