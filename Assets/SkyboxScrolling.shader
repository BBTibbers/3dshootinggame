Shader "Custom/SkyboxScrolling"
{
    Properties
    {
        _MainTex("Panoramic Texture", 2D) = "white" {}
        _ScrollSpeed("Scroll Speed", Vector) = (0.01, 0, 0, 0)
    }
    SubShader
    {
        Tags { "Queue" = "Background" }
        Cull Off
        ZWrite Off
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ScrollSpeed;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 offset = _ScrollSpeed.xy * _Time.y;
                return tex2D(_MainTex, i.uv + offset);
            }
            ENDCG
        }
    }
}
