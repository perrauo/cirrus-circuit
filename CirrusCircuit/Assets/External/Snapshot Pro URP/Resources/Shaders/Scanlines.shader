Shader "SnapshotProURP/Scanlines"
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
			sampler2D _ScanlineTex;
			int _Size;
			float _Strength;

            float4 frag (v2f_img i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;

				float2 scanlineUV = i.uv * _ScreenParams.xy / _Size;
				float3 scanlines = tex2D(_ScanlineTex, scanlineUV).xyz;

				col = lerp(col, col * scanlines, _Strength);

				return float4(col, 1.0f);
            }
            ENDCG
        }
    }
}
