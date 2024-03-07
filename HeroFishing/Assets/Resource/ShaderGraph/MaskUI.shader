Shader"UI/MaskUI"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _AlphaTex ("Alpha Tex", 2D) = "white" {}
   
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalPipeline"
        }

//        Stencil
//        {
//            Ref[ _Stencil]
//            Comp [_StencilComp]
//            Pass [_StencilOp]
//ReadMask[ _StencilReadMask]
//            WriteMask [_StencilWriteMask]
//        }

        Cull Off

        Lighting Off

        ZWrite Off

        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        //ColorMask[_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #pragma multi_compile __ UNITY_UI_ALPHACLIP
    
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;

                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            fixed4 _Color;
            fixed4 _TextureSampleAdd;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.color = IN.color * _Color;
                OUT.uv = IN.texcoord;
                OUT.uv1 = IN.texcoord1;
                return OUT;
            }

            sampler2D _MainTex;
            fixed4 _MainTex_ST;
            sampler2D _AlphaTex;
            fixed4 _AlphaTex_ST;

            fixed4 frag(v2f IN) : SV_Target
            {
                float4 color = (tex2D(_MainTex, IN.uv * _MainTex_ST.xy + _MainTex_ST.zw) + _TextureSampleAdd) * IN.color;
                const float mask_alpha = (tex2D(_AlphaTex, IN.uv1 * _AlphaTex_ST.xy + _AlphaTex_ST.zw) + _TextureSampleAdd).a;
                color.a *= mask_alpha;
                return color;
            }
            ENDCG
        }
    }
}
