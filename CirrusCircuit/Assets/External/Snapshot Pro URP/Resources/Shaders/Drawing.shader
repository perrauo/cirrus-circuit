Shader "SnapshotProURP/Drawing"
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
			sampler2D _DrawingTex;
			sampler2D _CameraDepthTexture;
			float _OverlayOffset;
			float _Strength;
			float _Tiling;
			float _Smudge;
			float _DepthThreshold;

            float4 frag (v2f_img i) : SV_Target
            {
				float2 drawingUV = (i.uv + _OverlayOffset) * _Tiling;
				drawingUV.y *= _ScreenParams.y / _ScreenParams.x;

				float3 drawingCol = (tex2D(_DrawingTex, drawingUV).xyz + 
					tex2D(_DrawingTex, drawingUV / 3.0f).xyz) / 2.0f;

				float2 texUV = i.uv + drawingCol * _Smudge;
				float3 col = tex2D(_MainTex, texUV).xyz;

				float lum = dot(col, float3(0.3f, 0.59f, 0.11f));
				float3 drawing = lerp(col, drawingCol * col, (1.0f - lum) * _Strength);

				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				depth = Linear01Depth(depth);

				return float4(depth < _DepthThreshold ? drawing : col, 1.0f);
            }
            ENDCG
        }
    }
}
