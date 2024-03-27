Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _Width("Width", Float) = 0.1
        _Degree("Degree", Float) = 30
        _Speed("Speed", Float) = 1
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
Name"Universal Forward"
            Tags
{
                // LightMode: <None>
}
        
        // Render State
Cull Back

Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

ZTest Always

ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>
        
        // Defines
        
#define ATTRIBUTES_NEED_NORMAL
#define ATTRIBUTES_NEED_TANGENT
#define ATTRIBUTES_NEED_TEXCOORD0
#define ATTRIBUTES_NEED_COLOR
#define VARYINGS_NEED_POSITION_WS
#define VARYINGS_NEED_NORMAL_WS
#define VARYINGS_NEED_TEXCOORD0
#define VARYINGS_NEED_COLOR
#define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
#define SHADERPASS SHADERPASS_UNLIT
#define _FOG_FRAGMENT 1
#define _SURFACE_TYPE_TRANSPARENT 1
#define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
struct Attributes
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float4 uv0 : TEXCOORD0;
    float4 color : COLOR;
#if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
#endif
};
struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 positionWS;
    float3 normalWS;
    float4 texCoord0;
    float4 color;
#if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
};
struct SurfaceDescriptionInputs
{
    float4 uv0;
    float4 VertexColor;
    float3 TimeParameters;
};
struct VertexDescriptionInputs
{
    float3 ObjectSpaceNormal;
    float3 ObjectSpaceTangent;
    float3 ObjectSpacePosition;
};
struct PackedVaryings
{
    float4 positionCS : SV_POSITION;
    float4 texCoord0 : INTERP0;
    float4 color : INTERP1;
    float3 positionWS : INTERP2;
    float3 normalWS : INTERP3;
#if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
};
        
PackedVaryings PackVaryings(Varyings input)
{
    PackedVaryings output;
    ZERO_INITIALIZE(PackedVaryings, output);
    output.positionCS = input.positionCS;
    output.texCoord0.xyzw = input.texCoord0;
    output.color.xyzw = input.color;
    output.positionWS.xyz = input.positionWS;
    output.normalWS.xyz = input.normalWS;
#if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
#endif
    return output;
}
        
Varyings UnpackVaryings(PackedVaryings input)
{
    Varyings output;
    output.positionCS = input.positionCS;
    output.texCoord0 = input.texCoord0.xyzw;
    output.color = input.color.xyzw;
    output.positionWS = input.positionWS.xyz;
    output.normalWS = input.normalWS.xyz;
#if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
#endif
    return output;
}
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
float4 _MainTex_TexelSize;
float _Width;
float _Degree;
float _Speed;
CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
#ifdef SCENEPICKINGPASS
        float4 _SelectionID;
#endif
        
        // -- Properties used by SceneSelectionPass
#ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
#endif
        
        // Graph Functions
        
void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}
        
void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
            //rotation matrix
    Rotation = Rotation * (3.1415926f / 180.0f);
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);
        
            //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;
        
            //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;
        
    Out = UV;
}
        
void Unity_Divide_float(float A, float B, out float Out)
{
    Out = A / B;
}
        
void Unity_Multiply_float_float(float A, float B, out float Out)
{
    Out = A * B;
}
        
void Unity_Add_float(float A, float B, out float Out)
{
    Out = A + B;
}
        
void SquareWave_float(float In, out float Out)
{
    Out = 1.0 - 2.0 * round(frac(In));
}
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
struct VertexDescription
{
    float3 Position;
    float3 Normal;
    float3 Tangent;
};
        
VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
    VertexDescription description = (VertexDescription) 0;
    description.Position = IN.ObjectSpacePosition;
    description.Normal = IN.ObjectSpaceNormal;
    description.Tangent = IN.ObjectSpaceTangent;
    return description;
}
        
        // Custom interpolators, pre surface
#ifdef FEATURES_GRAPH_VERTEX
Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
{
    return output;
}
#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
#endif
        
        // Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
    float AlphaClipThreshold;
};
        
SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription) 0;
    UnityTexture2D _Property_d5554c0a5c114d97943f471b4f8ea79b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
    float4 _SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_d5554c0a5c114d97943f471b4f8ea79b_Out_0_Texture2D.tex, _Property_d5554c0a5c114d97943f471b4f8ea79b_Out_0_Texture2D.samplerstate, _Property_d5554c0a5c114d97943f471b4f8ea79b_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
    float _SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_R_4_Float = _SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_RGBA_0_Vector4.r;
    float _SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_G_5_Float = _SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_RGBA_0_Vector4.g;
    float _SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_B_6_Float = _SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_RGBA_0_Vector4.b;
    float _SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_A_7_Float = _SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_RGBA_0_Vector4.a;
    float4 _Multiply_ab83e105926c431c92c4f03d92c6415f_Out_2_Vector4;
    Unity_Multiply_float4_float4(IN.VertexColor, _SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_RGBA_0_Vector4, _Multiply_ab83e105926c431c92c4f03d92c6415f_Out_2_Vector4);
    float4 _UV_272c8becda6447718dc947d3f465a2f2_Out_0_Vector4 = IN.uv0;
    float _Property_f9cf1cae839e490cad94b02ea0961b23_Out_0_Float = _Degree;
    float2 _Rotate_50d0d3ec52e74c87bcf1d50c953e9cd5_Out_3_Vector2;
    Unity_Rotate_Degrees_float((_UV_272c8becda6447718dc947d3f465a2f2_Out_0_Vector4.xy), float2(0.5, 0.5), _Property_f9cf1cae839e490cad94b02ea0961b23_Out_0_Float, _Rotate_50d0d3ec52e74c87bcf1d50c953e9cd5_Out_3_Vector2);
    float _Split_aec5d0f8965746c9a58bd8f7ccac052c_R_1_Float = _Rotate_50d0d3ec52e74c87bcf1d50c953e9cd5_Out_3_Vector2[0];
    float _Split_aec5d0f8965746c9a58bd8f7ccac052c_G_2_Float = _Rotate_50d0d3ec52e74c87bcf1d50c953e9cd5_Out_3_Vector2[1];
    float _Split_aec5d0f8965746c9a58bd8f7ccac052c_B_3_Float = 0;
    float _Split_aec5d0f8965746c9a58bd8f7ccac052c_A_4_Float = 0;
    float _Property_9e3ae3b1edee4ee084d8745cf911da19_Out_0_Float = _Width;
    float _Divide_9fb96b2c34364ee99eacb0893137498a_Out_2_Float;
    Unity_Divide_float(1, _Property_9e3ae3b1edee4ee084d8745cf911da19_Out_0_Float, _Divide_9fb96b2c34364ee99eacb0893137498a_Out_2_Float);
    float _Multiply_c9e1158ab1df4c038836b0d70d9f5b2e_Out_2_Float;
    Unity_Multiply_float_float(_Split_aec5d0f8965746c9a58bd8f7ccac052c_R_1_Float, _Divide_9fb96b2c34364ee99eacb0893137498a_Out_2_Float, _Multiply_c9e1158ab1df4c038836b0d70d9f5b2e_Out_2_Float);
    float _Property_583673665b644e6d989cfd05a941450c_Out_0_Float = _Speed;
    float _Multiply_d9568a5329a242df820db784121660e4_Out_2_Float;
    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_583673665b644e6d989cfd05a941450c_Out_0_Float, _Multiply_d9568a5329a242df820db784121660e4_Out_2_Float);
    float _Add_c98f5a7afa6745c3af536b8405c4229f_Out_2_Float;
    Unity_Add_float(_Multiply_c9e1158ab1df4c038836b0d70d9f5b2e_Out_2_Float, _Multiply_d9568a5329a242df820db784121660e4_Out_2_Float, _Add_c98f5a7afa6745c3af536b8405c4229f_Out_2_Float);
    float _SquareWave_8fe99099276c4b5a80a0cd398b5a0f98_Out_1_Float;
    SquareWave_float(_Add_c98f5a7afa6745c3af536b8405c4229f_Out_2_Float, _SquareWave_8fe99099276c4b5a80a0cd398b5a0f98_Out_1_Float);
    float4 _Multiply_57fdb58358ae444ba498011ac3c78bc6_Out_2_Vector4;
    Unity_Multiply_float4_float4(_Multiply_ab83e105926c431c92c4f03d92c6415f_Out_2_Vector4, (_SquareWave_8fe99099276c4b5a80a0cd398b5a0f98_Out_1_Float.xxxx), _Multiply_57fdb58358ae444ba498011ac3c78bc6_Out_2_Vector4);
    float _Split_acc7eabac91344b5b1527e790ae9bef9_R_1_Float = IN.VertexColor[0];
    float _Split_acc7eabac91344b5b1527e790ae9bef9_G_2_Float = IN.VertexColor[1];
    float _Split_acc7eabac91344b5b1527e790ae9bef9_B_3_Float = IN.VertexColor[2];
    float _Split_acc7eabac91344b5b1527e790ae9bef9_A_4_Float = IN.VertexColor[3];
    float _Multiply_d9202354c66747b7bdcb1bd6c333a4c7_Out_2_Float;
    Unity_Multiply_float_float(_SampleTexture2D_8f849c6f56ed4750a0983b6533908c45_A_7_Float, _Split_acc7eabac91344b5b1527e790ae9bef9_A_4_Float, _Multiply_d9202354c66747b7bdcb1bd6c333a4c7_Out_2_Float);
    float _Multiply_35f2e2f7fdee4915a15454100b81af3d_Out_2_Float;
    Unity_Multiply_float_float(_Multiply_d9202354c66747b7bdcb1bd6c333a4c7_Out_2_Float, _SquareWave_8fe99099276c4b5a80a0cd398b5a0f98_Out_1_Float, _Multiply_35f2e2f7fdee4915a15454100b81af3d_Out_2_Float);
    surface.BaseColor = (_Multiply_57fdb58358ae444ba498011ac3c78bc6_Out_2_Vector4.xyz);
    surface.Alpha = _Multiply_35f2e2f7fdee4915a15454100b81af3d_Out_2_Float;
    surface.AlphaClipThreshold = 0.5;
    return surface;
}
        
        // --------------------------------------------------
        // Build Graph Inputs
#ifdef HAVE_VFX_MODIFICATION
#define VFX_SRP_ATTRIBUTES Attributes
#define VFX_SRP_VARYINGS Varyings
#define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
#endif
VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;
        
    return output;
}
SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
#ifdef HAVE_VFX_MODIFICATION
#if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
#endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
#endif
        
            
        
        
        
        
        
        
#if UNITY_UV_STARTS_AT_TOP
#else
#endif
        
        
    output.uv0 = input.texCoord0;
    output.VertexColor = input.color;
    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
    return output;
}
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        
    }
CustomEditor"UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline"UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
FallBack"Hidden/Shader Graph/FallbackError"
}