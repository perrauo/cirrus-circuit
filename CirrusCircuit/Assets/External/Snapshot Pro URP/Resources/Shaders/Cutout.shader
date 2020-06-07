Shader "SnapshotProURP/Cutout"
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
			sampler2D _CutoutTex;
			float4 _BorderColor;
			int _Stretch;

            float4 frag (v2f_img i) : SV_Target
            {
				float aspect = _ScreenParams.x / _ScreenParams.y;
				float2 nonStretchedUVs = float2(aspect * (i.uv.x - 0.5f) + 0.5f, i.uv.y);

				float2 cutoutUVs = (_Stretch == 0) ? nonStretchedUVs : i.uv;
				float cutoutAlpha = tex2D(_CutoutTex, cutoutUVs).a;
				float4 col = tex2D(_MainTex, i.uv);
				return lerp(col, _BorderColor, cutoutAlpha);
            }
            ENDCG
        }
    }
}
