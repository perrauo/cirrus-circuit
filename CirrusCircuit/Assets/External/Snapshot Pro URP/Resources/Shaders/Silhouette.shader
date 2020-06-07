Shader "SnapshotProURP/Silhouette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float4 _NearColor;
			float4 _FarColor;

            float4 frag (v2f_img i) : SV_Target
            {
				float depth = tex2D(_CameraDepthTexture, i.uv);
				depth = Linear01Depth(depth);

				return lerp(_NearColor, _FarColor, depth);
            }
            ENDCG
        }
    }
}
