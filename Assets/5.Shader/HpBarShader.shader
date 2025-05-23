Shader "Custom/HpsBar"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _LineCount ("Number of Lines", int) = 10
        _FilledLines ("Filled Lines", int) = 3
        _FillColor ("Fill Color", Color) = (1, 0, 0, 1)
        _BackgroundColor ("Background Color", Color) = (0.2, 0.2, 0.2, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _LineCount;
            int _FilledLines;
            float4 _FillColor;
            float4 _BackgroundColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float lineWidth = 1.0 / _LineCount;
                float lineIndex = floor(uv.x / lineWidth);
                bool isFilled = (lineIndex < _FilledLines);

                fixed4 texColor = tex2D(_MainTex, uv);
                if (isFilled)
                {
                    return texColor * _FillColor;
                }
                else
                {
                    return fixed4(0, 0, 0, 0);
                }
            }
            ENDCG
        }
    }
}
