Shader "SnapshotProURP/Greyscale"
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
			float _Strength;

            float4 frag (v2f_img i) : SV_Target
            {
				float4 col = tex2D(_MainTex, i.uv);
				col.rgb = lerp(col.rgb, dot(col.rgb, float3(0.3f, 0.59f, 0.11f)), _Strength);
                return col;
            }
            ENDCG
        }
    }
}
