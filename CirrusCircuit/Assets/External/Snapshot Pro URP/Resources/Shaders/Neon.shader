Shader "SnapshotProURP/Neon"
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
			sampler2D _CameraDepthNormalsTexture;

			float4 _OutlineColor;
			float _ColorSensitivity;
			float _ColorStrength;
			float _DepthSensitivity;
			float _DepthStrength;
			float _NormalsSensitivity;
			float _NormalsStrength;

			float _DepthThreshold;

			float _SaturationFloor;
			float _LightnessFloor;

			// Credit to https://alexanderameye.github.io/outlineshader.html:
			float3 DecodeNormal(float4 enc)
			{
				float kScale = 1.7777;
				float3 nn = enc.xyz*float3(2 * kScale, 2 * kScale, 0) + float3(-kScale, -kScale, 1);
				float g = 2.0 / dot(nn.xyz, nn.xyz);
				float3 n;
				n.xy = g * nn.xy;
				n.z = g - 1;
				return n;
			}

			// Credit: http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
			float3 rgb2hsv(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = c.g < c.b ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
				float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			// Credit: http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
			float3 hsv2rgb(float3 c)
			{
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
			}


            float4 frag (v2f_img i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;

				float2 leftUV = i.uv + float2(1.0f / -_ScreenParams.x, 0.0f);
				float2 rightUV = i.uv + float2(1.0f / _ScreenParams.x, 0.0f);
				float2 bottomUV = i.uv + float2(0.0f, 1.0f / -_ScreenParams.y);
				float2 topUV = i.uv + float2(0.0f, 1.0f / _ScreenParams.y);

				float3 col0 = tex2D(_MainTex, leftUV).xyz;
				float3 col1 = tex2D(_MainTex, rightUV).xyz;
				float3 col2 = tex2D(_MainTex, bottomUV).xyz;
				float3 col3 = tex2D(_MainTex, topUV).xyz;

				float3 c0 = col1 - col0;
				float3 c1 = col3 - col2;

				float edgeCol = sqrt(dot(c0, c0) + dot(c1, c1));
				edgeCol = edgeCol > _ColorSensitivity ? _ColorStrength : 0;

				float depth0 = tex2D(_CameraDepthTexture, leftUV).r;
				float depth1 = tex2D(_CameraDepthTexture, rightUV).r;
				float depth2 = tex2D(_CameraDepthTexture, bottomUV).r;
				float depth3 = tex2D(_CameraDepthTexture, topUV).r;

				depth0 = Linear01Depth(depth0);
				depth1 = Linear01Depth(depth1);
				depth2 = Linear01Depth(depth2);
				depth3 = Linear01Depth(depth3);

				float d0 = depth1 - depth0;
				float d1 = depth3 - depth2;

				float edgeDepth = sqrt(d0 * d0 + d1 * d1);
				edgeDepth = edgeDepth > _DepthSensitivity ? _DepthStrength : 0;

				float3 normal0 = DecodeNormal(tex2D(_CameraDepthNormalsTexture, leftUV));
				float3 normal1 = DecodeNormal(tex2D(_CameraDepthNormalsTexture, rightUV));
				float3 normal2 = DecodeNormal(tex2D(_CameraDepthNormalsTexture, bottomUV));
				float3 normal3 = DecodeNormal(tex2D(_CameraDepthNormalsTexture, topUV));

				float n0 = normal1 - normal0;
				float3 n1 = normal3 - normal2;

				float edgeNormal = sqrt(dot(n0, n0) + dot(n1, n1));
				edgeNormal = edgeNormal > _NormalsSensitivity ? _NormalsStrength : 0;

				float edge = max(max(edgeCol, edgeDepth), edgeNormal);

				float depth = tex2D(_CameraDepthTexture, i.uv);
				depth = Linear01Depth(depth);
				edge = depth > _DepthThreshold ? 0.0f : edge;

				float3 hsvTex = rgb2hsv(col);
				hsvTex.y = max(hsvTex.y, _SaturationFloor);
				hsvTex.z = max(hsvTex.z, _LightnessFloor);
				col = hsv2rgb(hsvTex);

				return float4(col * edge, 1.0f);
            }
            ENDCG
        }
    }
}
