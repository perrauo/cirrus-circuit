Shader "SnapshotProURP/FilmBars"
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
			float _Aspect;

            float4 frag (v2f_img i) : SV_Target
            {
				float4 col = tex2D(_MainTex, i.uv);
				float aspect = _ScreenParams.x / _ScreenParams.y;
				float bars = step(abs(0.5f - i.uv.y) * 2.0f, aspect / _Aspect);

                return col * bars;
            }
            ENDCG
        }
    }
}
