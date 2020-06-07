Shader "SnapshotProURP/Underwater"
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
			sampler2D _CameraDepthTexture;
			sampler2D _BumpMap;
			float _Strength;
			float4 _WaterColor;
			float _FogStrength;

            float4 frag (v2f_img i) : SV_Target
            {
				float2 timeUV = (i.uv + _Time.x) % 1.0f;
				float4 bumpTex = tex2D(_BumpMap, timeUV);
				float2 normal = bumpTex.wy * 2.0f - 1.0f;

				float2 normalUV = i.uv + (1.0f / _ScreenParams.xy) * normal.xy * _Strength;
				float3 col = tex2D(_MainTex, normalUV).xyz;

				float3 depthTex = tex2D(_CameraDepthTexture, i.uv).xyz;
				float depth = Linear01Depth(depthTex.r);

				col = lerp(col, _WaterColor.xyz, depth * _FogStrength);
				return float4(col, 1.0f);
            }
            ENDCG
        }
    }
}
