﻿Shader "Custom/CopyShadowMap"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _ShadowMapTexture;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 screenuv : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenuv = ComputeScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_ShadowMapTexture, i.uv);
                return col;
                /*float2 uv = i.screenuv.xy / i.screenuv.w;
                float depth = 1 - Linear01Depth(SAMPLE_DEPTH_TEXTURE(_ShadowMapTexture, uv));
                return fixed4(depth, depth, depth, 1);*/
            }
            ENDCG
        }
    }
}