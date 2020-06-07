Shader "SnapshotProURP/Glitch"
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
			sampler2D _OffsetTex;
			float _OffsetStrength;
			float _VerticalTiling;

            float4 frag (v2f_img i) : SV_Target
            {
				float2 uv = i.uv;

				float offset = tex2D(_OffsetTex, float2(i.uv.x, i.uv.y * _VerticalTiling));
				uv.x += (offset - 0.5f) * _OffsetStrength + 1.0f;
				uv.x %= 1;

				return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
