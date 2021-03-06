// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SeganX/UI/Wave"
{
    Properties
    {
        _MainTex("Sprite (RGB)", 2D) = "white" {}
        _SurfTex("Wave (RGB)", 2D) = "black" {}
        _SurfParam("Wave Speed & Depth", Vector) = (0, 0, 0, 0)

        [Enum(ON,1,OFF,0)]	_ZWrite("Z Write", Int) = 0
        [Enum(BACK,2,FRONT,1,OFF,0)]	_Cull("Cull", Int) = 2
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendSrc("SrcFactor", Int) = 5
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendDest("DstFactor", Int) = 10

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull[_Cull]
            ZWrite[_ZWrite]
            Blend[_BlendSrc][_BlendDest]

            Lighting Off
            ZTest[unity_GUIZTestMode]
            ColorMask[_ColorMask]

            Pass
            {
                Name "FORWARD"
                Tags { "LightMode" = "ForwardBase" }

                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct vs_in {
                    float4 pos : POSITION;
                    fixed4 col : COLOR;
                    float2 uv0 : TEXCOORD0;
                };

                struct vs_out {
                    float4 pos : SV_POSITION;
                    fixed4 col : COLOR;
                    float2 uv0 : TEXCOORD0;
                    float2 uv1 : TEXCOORD1;
                };

                sampler2D _MainTex;
                sampler2D _SurfTex;
                float4 _MainTex_ST;
                float4 _SurfTex_ST;
                float4 _SurfParam;

                vs_out vert(vs_in v)
                {
                    vs_out o;
                    o.pos = UnityObjectToClipPos(v.pos);
                    o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                    o.uv1 = TRANSFORM_TEX(v.uv0, _SurfTex) + _Time.x * _SurfParam.xy;
                    //o.uv2 = TRANSFORM_TEX(v.uv0, _SurfTex) + _Time.x * _SurfSpeed.zw;
                    o.col = v.col;
                    return o;
                }

                fixed4 frag(vs_out i) : SV_Target
                {
                    fixed2 s = tex2D(_SurfTex, i.uv1).rg * 2 - 1;
                    s *= _SurfParam.wz;
                    fixed4 c = tex2D(_MainTex, i.uv0 + s) * i.col;
                    return c;
                }

                ENDCG
            }
        }

}
