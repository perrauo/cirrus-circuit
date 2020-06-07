Shader "SnapshotProURP/Mosaic"
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
			sampler2D _OverlayTex;
			float4 _OverlayColor;
			int _XTileCount;
			int _YTileCount;

            float4 frag (v2f_img i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;

				float2 overlayUV = i.uv * float2(_XTileCount, _YTileCount);
				float4 overlayCol = tex2D(_OverlayTex, overlayUV) * _OverlayColor;

				col = lerp(col, overlayCol.xyz, overlayCol.a);

				return float4(col, 1.0f);
            }
            ENDCG
        }
    }
}
