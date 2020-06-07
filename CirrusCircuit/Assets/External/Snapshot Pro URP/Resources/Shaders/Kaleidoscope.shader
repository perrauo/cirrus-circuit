Shader "SnapshotProURP/Kaleidoscope"
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
			float _SegmentCount;

			static const float PI = 3.14159265f;

            float4 frag (v2f_img i) : SV_Target
            {
				float2 shiftUV = _ScreenParams.xy * (i.uv - 0.5f);

				float radius = sqrt(dot(shiftUV, shiftUV));
				float angle = atan2(shiftUV.y, shiftUV.x);

				float segmentAngle = PI * 2.0f / _SegmentCount;
				angle -= segmentAngle * floor(angle / segmentAngle);
				angle = min(angle, segmentAngle - angle);

				float2 uv = float2(cos(angle), sin(angle)) * radius + _ScreenParams.xy / 2.0f;
				uv = max(min(uv, _ScreenParams.xy * 2.0f - uv), -uv) / _ScreenParams.xy;

				return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
