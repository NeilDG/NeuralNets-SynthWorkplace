// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlbedoShader"
{
        SubShader{
            // markers that specify that we don't need culling 
            // or comparing/writing to the depth buffer
            Cull Off
            ZWrite Off
            ZTest Always

            Pass{
                CGPROGRAM
                //include useful shader functions
                #include "UnityCG.cginc"

                //define vertex and fragment shader
                #pragma vertex vert
                #pragma fragment frag

                //the rendered screen so far
                sampler2D _CameraGBufferTexture0;
                sampler2D _CameraGBufferTexture1;
                sampler2D _CameraGBufferTexture2;

                //matrix to convert from view space to world space
                float4x4 _viewToWorld;

                //the object data that's put into the vertex shader
                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                //the data that's used to generate fragments and can be read by the fragment shader
                struct v2f {
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                //the vertex shader
                v2f vert(appdata v) {
                    v2f o;
                    //convert the vertex positions from object space to clip space so they can be rendered
                    o.position = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                //the fragment shader
                fixed4 frag(v2f i) : SV_TARGET{
                    float3 albedo = tex2D(_CameraGBufferTexture0, i.uv).rgb;
                    float alpha = tex2D(_CameraGBufferTexture0, i.uv).a;
                    return float4(albedo, alpha);
                }
                ENDCG
    }
        }
}
