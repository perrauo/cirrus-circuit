Shader "SnapshotProURP/SNES"
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
			int _BandingLevels;

			static const float EPS = 1e-10;

            float4 frag (v2f_img i) : SV_Target
            {
				float4 col = tex2D(_MainTex, i.uv);

				int r = (col.r - EPS) * _BandingLevels;
				int g = (col.g - EPS) * _BandingLevels;
				int b = (col.b - EPS) * _BandingLevels;

				float divisor = _BandingLevels - 1.0f;

                return float4(r / divisor, g / divisor, b / divisor, 1.0f);
            }
            ENDCG
        }
    }
}
