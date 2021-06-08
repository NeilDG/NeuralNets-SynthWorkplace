
Shader "Custom/DirectionalityMap"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #include "UnityCG.cginc"

            struct v2f {
                 float4 pos : SV_POSITION;
                 float3 normal : TEXCOORD0;
                 float3 viewT : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = normalize(v.normal);
                o.viewT = normalize(WorldSpaceViewDir(v.vertex));//ObjSpaceViewDir is similar, but localspace.
                return o;
            }
            ENDCG
        }
    }
}