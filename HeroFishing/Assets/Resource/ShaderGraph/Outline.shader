Shader"UI/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" }
        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityUI.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _MainTex_TexelSize;

            float4 _ClipRect;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float4 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 tangent : TANGENT;
                float4 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float4 vertex : SV_POSITION;
                float4 worldPosition : TEXCOORD4;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.tangent = v.tangent;
                o.texcoord = v.texcoord;
                o.uv1 = v.uv1;
                o.uv2 = v.uv2;
                o.uv3 = v.uv3;
                o.normal = v.normal;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 IsInRect(float2 pPos, float2 pClipRectMin, float2 pClipRectMax) {
                pPos = step(pClipRectMin, pPos) * step(pPos, pClipRectMax);
                return pPos.x * pPos.y;
            }

            fixed SampleAlpha(int pIndex, v2f i){
                const fixed sinArray[12] = { 0, 0.5, 0.866, 1, 0.866, 0.5, 0, -0.5, -0.866, -1, -0.866, -0.5 };
                const fixed cosArray[12] = { 1, 0.866, 0.5, 0, -0.5, -0.866, -1, -0.866, -0.5, 0, 0.5, 0.866 };
                float2 pos = i.texcoord + _MainTex_TexelSize.xy * float2(cosArray[pIndex], sinArray[pIndex]) * i.normal.z;
                return IsInRect(pos, i.uv1, i.uv2) * (tex2D(_MainTex, pos) + _TextureSampleAdd).w * i.tangent.w;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 color = (tex2D(_MainTex, i.texcoord) + _TextureSampleAdd) * i.color;
                if(i.normal.z > 0){
                    color.w *= IsInRect(i.texcoord, i.uv1, i.uv2);
                    half4 val = half4(i.uv3.x, i.uv3.y, i.tangent.z, 0);
                    val.w += SampleAlpha(0, i);
                    val.w += SampleAlpha(1, i);
                    val.w += SampleAlpha(2, i);
                    val.w += SampleAlpha(3, i);
                    val.w += SampleAlpha(4, i);
                    val.w += SampleAlpha(5, i);
                    val.w += SampleAlpha(6, i);
                    val.w += SampleAlpha(7, i);
                    val.w += SampleAlpha(8, i);
                    val.w += SampleAlpha(9, i);
                    val.w += SampleAlpha(10, i);
                    val.w += SampleAlpha(11, i);
        
                    color = (val * (1.0 - color.a)) + (color * color.a);
                    color.a = saturate(color.a);
                    color.a *= i.color.a * i.color.a * i.color.a;
                }
    
                color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
    #ifdef UNITY_UI_ALPHACLIP
                clip(color.abort - 0.001);
#endif
                return color;
            }
            ENDCG
        }
    }
}
