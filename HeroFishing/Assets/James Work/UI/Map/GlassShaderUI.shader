Shader"Unlit/GlassShaderUI"
{
    Properties
    {
        _albedo("albedo", Color) = (1, 1, 1, 1)
        _offset("offset", Vector) = (0, -0.83, 0, 0)
        _tiling("tiling", Vector) = (0.5, 0.5, 0, 0)
        [HDR]_egde_color("egde color", Color) = (0.6603774, 0.6603774, 0.6603774, 1)
        _edge_glow("edge glow", Float) = 1
        [NoScaleOffset]_Texture2D("Texture2D", 2D) = "white" {}
        _Opacity("Opacity", Range(0, 1)) = 0.5
        [HDR]_Color("Color", Color) = (2.082829, 2.479064, 3.003808, 1)
        _Smooth("Smooth", Float) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0.5
        _Distort_move("Distort move", Vector) = (0, 0.5, 0, 0)
        [PerRendererData]_MainTex("MainTex", 2D) = "white" {}
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
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalLitSubTarget"
        }
        Pass
        {
            Name"Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Off

        Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
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
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile_fragment _ _SHADOWS_SOFT_LOW
        #pragma multi_compile_fragment _ _SHADOWS_SOFT_MEDIUM
        #pragma multi_compile_fragment _ _SHADOWS_SOFT_HIGH
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _LIGHT_COOKIES
        #pragma multi_compile _ _FORWARD_PLUS
        // GraphKeywords: <None>
        
        // Defines
        
#define _NORMALMAP 1
#define _NORMAL_DROPOFF_TS 1
#define ATTRIBUTES_NEED_NORMAL
#define ATTRIBUTES_NEED_TANGENT
#define ATTRIBUTES_NEED_TEXCOORD0
#define ATTRIBUTES_NEED_TEXCOORD1
#define ATTRIBUTES_NEED_TEXCOORD2
#define VARYINGS_NEED_POSITION_WS
#define VARYINGS_NEED_NORMAL_WS
#define VARYINGS_NEED_TANGENT_WS
#define VARYINGS_NEED_TEXCOORD0
#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
#define VARYINGS_NEED_SHADOW_COORD
#define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
#define SHADERPASS SHADERPASS_FORWARD
#define _FOG_FRAGMENT 1
#define _SURFACE_TYPE_TRANSPARENT 1
#define _ALPHAPREMULTIPLY_ON 1
#define _RECEIVE_SHADOWS_OFF 1
        
        
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
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
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
    float4 uv1 : TEXCOORD1;
    float4 uv2 : TEXCOORD2;
#if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
#endif
};
struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 positionWS;
    float3 normalWS;
    float4 tangentWS;
    float4 texCoord0;
#if defined(LIGHTMAP_ON)
             float2 staticLightmapUV;
#endif
#if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV;
#endif
#if !defined(LIGHTMAP_ON)
    float3 sh;
#endif
    float4 fogFactorAndVertexLight;
#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord;
#endif
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
    float3 WorldSpaceNormal;
    float3 TangentSpaceNormal;
    float3 WorldSpaceViewDirection;
    float4 uv0;
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
#if defined(LIGHTMAP_ON)
             float2 staticLightmapUV : INTERP0;
#endif
#if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV : INTERP1;
#endif
#if !defined(LIGHTMAP_ON)
    float3 sh : INTERP2;
#endif
#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord : INTERP3;
#endif
    float4 tangentWS : INTERP4;
    float4 texCoord0 : INTERP5;
    float4 fogFactorAndVertexLight : INTERP6;
    float3 positionWS : INTERP7;
    float3 normalWS : INTERP8;
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
#if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.staticLightmapUV;
#endif
#if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.dynamicLightmapUV;
#endif
#if !defined(LIGHTMAP_ON)
    output.sh = input.sh;
#endif
#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.shadowCoord;
#endif
    output.tangentWS.xyzw = input.tangentWS;
    output.texCoord0.xyzw = input.texCoord0;
    output.fogFactorAndVertexLight.xyzw = input.fogFactorAndVertexLight;
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
#if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.staticLightmapUV;
#endif
#if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.dynamicLightmapUV;
#endif
#if !defined(LIGHTMAP_ON)
    output.sh = input.sh;
#endif
#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.shadowCoord;
#endif
    output.tangentWS = input.tangentWS.xyzw;
    output.texCoord0 = input.texCoord0.xyzw;
    output.fogFactorAndVertexLight = input.fogFactorAndVertexLight.xyzw;
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
float4 _Texture2D_TexelSize;
float _Opacity;
float4 _albedo;
float4 _Color;
float _Smooth;
float _Metallic;
float2 _offset;
float2 _tiling;
float _edge_glow;
float4 _egde_color;
float2 _Distort_move;
float4 _MainTex_TexelSize;
CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
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
        
void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}
        
void Unity_Multiply_float_float(float A, float B, out float Out)
{
    Out = A * B;
}
        
void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}
        
void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
{
    Out = A * B;
}
        
void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}
        
float Unity_SimpleNoise_ValueNoise_LegacySine_float(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);
    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0;
    Hash_LegacySine_2_1_float(c0, r0);
    float r1;
    Hash_LegacySine_2_1_float(c1, r1);
    float r2;
    Hash_LegacySine_2_1_float(c2, r2);
    float r3;
    Hash_LegacySine_2_1_float(c3, r3);
    float bottomOfGrid = lerp(r0, r1, f.x);
    float topOfGrid = lerp(r2, r3, f.x);
    float t = lerp(bottomOfGrid, topOfGrid, f.y);
    return t;
}
        
void Unity_SimpleNoise_LegacySine_float(float2 UV, float Scale, out float Out)
{
    float freq, amp;
    Out = 0.0f;
    freq = pow(2.0, float(0));
    amp = pow(0.5, float(3 - 0));
    Out += Unity_SimpleNoise_ValueNoise_LegacySine_float(float2(UV.xy * (Scale / freq))) * amp;
    freq = pow(2.0, float(1));
    amp = pow(0.5, float(3 - 1));
    Out += Unity_SimpleNoise_ValueNoise_LegacySine_float(float2(UV.xy * (Scale / freq))) * amp;
    freq = pow(2.0, float(2));
    amp = pow(0.5, float(3 - 2));
    Out += Unity_SimpleNoise_ValueNoise_LegacySine_float(float2(UV.xy * (Scale / freq))) * amp;
}
        
void Unity_Blend_Overlay_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Base, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}
        
void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
    Out = A + B;
}
        
float2 Unity_Voronoi_RandomVector_Deterministic_float(float2 UV, float offset)
{
    Hash_Tchou_2_2_float(UV, UV);
    return float2(sin(UV.y * offset), cos(UV.x * offset)) * 0.5 + 0.5;
}
        
void Unity_Voronoi_Deterministic_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
{
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float t = 8.0;
    float3 res = float3(8.0, 0.0, 0.0);
    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            float2 lattice = float2(x, y);
            float2 offset = Unity_Voronoi_RandomVector_Deterministic_float(lattice + g, AngleOffset);
            float d = distance(lattice + offset, f);
            if (d < res.x)
            {
                res = float3(d, offset.x, offset.y);
                Out = res.x;
                Cells = res.y;
            }
        }
    }
}
        
void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
    Out = smoothstep(Edge1, Edge2, In);
}
        
void Unity_OneMinus_float(float In, out float Out)
{
    Out = 1 - In;
}
        
float Unity_SimpleNoise_ValueNoise_Deterministic_float(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);
    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0;
    Hash_Tchou_2_1_float(c0, r0);
    float r1;
    Hash_Tchou_2_1_float(c1, r1);
    float r2;
    Hash_Tchou_2_1_float(c2, r2);
    float r3;
    Hash_Tchou_2_1_float(c3, r3);
    float bottomOfGrid = lerp(r0, r1, f.x);
    float topOfGrid = lerp(r2, r3, f.x);
    float t = lerp(bottomOfGrid, topOfGrid, f.y);
    return t;
}
        
void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
{
    float freq, amp;
    Out = 0.0f;
    freq = pow(2.0, float(0));
    amp = pow(0.5, float(3 - 0));
    Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
    freq = pow(2.0, float(1));
    amp = pow(0.5, float(3 - 1));
    Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
    freq = pow(2.0, float(2));
    amp = pow(0.5, float(3 - 2));
    Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
}
        
void Unity_Clamp_float4(float4 In, float4 Min, float4 Max, out float4 Out)
{
    Out = clamp(In, Min, Max);
}
        
void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
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
    float3 NormalTS;
    float3 Emission;
    float Metallic;
    float Smoothness;
    float Occlusion;
    float Alpha;
};
        
SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription) 0;
    float _FresnelEffect_739c048fe3d04d3b8da77cef1cd3100a_Out_3_Float;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 5, _FresnelEffect_739c048fe3d04d3b8da77cef1cd3100a_Out_3_Float);
    float _Property_71129c5cbb07496599ecc95db1df291e_Out_0_Float = _edge_glow;
    float _Multiply_65403144005f4eb3847dc908c744dfe8_Out_2_Float;
    Unity_Multiply_float_float(_FresnelEffect_739c048fe3d04d3b8da77cef1cd3100a_Out_3_Float, _Property_71129c5cbb07496599ecc95db1df291e_Out_0_Float, _Multiply_65403144005f4eb3847dc908c744dfe8_Out_2_Float);
    float4 _Property_b01667f9e0d84c568369900f484dbd29_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_egde_color) : _egde_color;
    float4 _Multiply_7af8452887134d268ecb368a2f91d9bc_Out_2_Vector4;
    Unity_Multiply_float4_float4((_Multiply_65403144005f4eb3847dc908c744dfe8_Out_2_Float.xxxx), _Property_b01667f9e0d84c568369900f484dbd29_Out_0_Vector4, _Multiply_7af8452887134d268ecb368a2f91d9bc_Out_2_Vector4);
    float4 _Property_c124cc32f3ec4161b36324f49c1e9eaf_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
    UnityTexture2D _Property_f463bbae2e3b4671b5681ccb71da58f3_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture2D);
    float2 _Property_ffe4c085e6ad40489d93e8e0a6e09d47_Out_0_Vector2 = _tiling;
    float2 _Property_449d8e6348024c5887a3c8665f26f1e2_Out_0_Vector2 = _offset;
    float2 _Multiply_e15656bbcbb64208830f46ddfcc8d26d_Out_2_Vector2;
    Unity_Multiply_float2_float2(_Property_449d8e6348024c5887a3c8665f26f1e2_Out_0_Vector2, (IN.TimeParameters.x.xx), _Multiply_e15656bbcbb64208830f46ddfcc8d26d_Out_2_Vector2);
    float2 _TilingAndOffset_f44cd4e0e64e44aa88a3a7c9fd118d4a_Out_3_Vector2;
    Unity_TilingAndOffset_float(IN.uv0.xy, _Property_ffe4c085e6ad40489d93e8e0a6e09d47_Out_0_Vector2, _Multiply_e15656bbcbb64208830f46ddfcc8d26d_Out_2_Vector2, _TilingAndOffset_f44cd4e0e64e44aa88a3a7c9fd118d4a_Out_3_Vector2);
    float4 _UV_8140444fbb6a4449909eacf83f92c452_Out_0_Vector4 = IN.uv0;
    float2 _Property_d495b7c8579f4b73841e38aee1204160_Out_0_Vector2 = _Distort_move;
    float2 _Multiply_2a68817a74054318b9778935c971188d_Out_2_Vector2;
    Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_d495b7c8579f4b73841e38aee1204160_Out_0_Vector2, _Multiply_2a68817a74054318b9778935c971188d_Out_2_Vector2);
    float2 _TilingAndOffset_31444a0725c543d29db971f21c9a96dd_Out_3_Vector2;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2(1, 1), _Multiply_2a68817a74054318b9778935c971188d_Out_2_Vector2, _TilingAndOffset_31444a0725c543d29db971f21c9a96dd_Out_3_Vector2);
    float _SimpleNoise_6000163dc80845a18a0d333f23910950_Out_2_Float;
    Unity_SimpleNoise_LegacySine_float(_TilingAndOffset_31444a0725c543d29db971f21c9a96dd_Out_3_Vector2, 10, _SimpleNoise_6000163dc80845a18a0d333f23910950_Out_2_Float);
    float4 _Blend_dfe1daa2f722461998528f4902f77488_Out_2_Vector4;
    Unity_Blend_Overlay_float4(_UV_8140444fbb6a4449909eacf83f92c452_Out_0_Vector4, (_SimpleNoise_6000163dc80845a18a0d333f23910950_Out_2_Float.xxxx), _Blend_dfe1daa2f722461998528f4902f77488_Out_2_Vector4, 0.5);
    float2 _Add_791f3e85ec344c6ebe8e2f28cf4113a3_Out_2_Vector2;
    Unity_Add_float2(_TilingAndOffset_f44cd4e0e64e44aa88a3a7c9fd118d4a_Out_3_Vector2, (_Blend_dfe1daa2f722461998528f4902f77488_Out_2_Vector4.xy), _Add_791f3e85ec344c6ebe8e2f28cf4113a3_Out_2_Vector2);
    float4 _SampleTexture2D_704f10b5705c4c93a19dd78bf007ba62_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f463bbae2e3b4671b5681ccb71da58f3_Out_0_Texture2D.tex, _Property_f463bbae2e3b4671b5681ccb71da58f3_Out_0_Texture2D.samplerstate, _Property_f463bbae2e3b4671b5681ccb71da58f3_Out_0_Texture2D.GetTransformedUV(_Add_791f3e85ec344c6ebe8e2f28cf4113a3_Out_2_Vector2));
    float _SampleTexture2D_704f10b5705c4c93a19dd78bf007ba62_R_4_Float = _SampleTexture2D_704f10b5705c4c93a19dd78bf007ba62_RGBA_0_Vector4.r;
    float _SampleTexture2D_704f10b5705c4c93a19dd78bf007ba62_G_5_Float = _SampleTexture2D_704f10b5705c4c93a19dd78bf007ba62_RGBA_0_Vector4.g;
    float _SampleTexture2D_704f10b5705c4c93a19dd78bf007ba62_B_6_Float = _SampleTexture2D_704f10b5705c4c93a19dd78bf007ba62_RGBA_0_Vector4.b;
    float _SampleTexture2D_704f10b5705c4c93a19dd78bf007ba62_A_7_Float = _SampleTexture2D_704f10b5705c4c93a19dd78bf007ba62_RGBA_0_Vector4.a;
    float4 _Multiply_da6f4da5d7d54660901a87fbe892036a_Out_2_Vector4;
    Unity_Multiply_float4_float4(_Property_c124cc32f3ec4161b36324f49c1e9eaf_Out_0_Vector4, _SampleTexture2D_704f10b5705c4c93a19dd78bf007ba62_RGBA_0_Vector4, _Multiply_da6f4da5d7d54660901a87fbe892036a_Out_2_Vector4);
    float2 _Vector2_a617b40c935548e190d9694f98c6a084_Out_0_Vector2 = float2(0, 0.5);
    float2 _Multiply_c94f7a1bfe8d4b25925ea91ed5fd2162_Out_2_Vector2;
    Unity_Multiply_float2_float2(_Vector2_a617b40c935548e190d9694f98c6a084_Out_0_Vector2, (IN.TimeParameters.x.xx), _Multiply_c94f7a1bfe8d4b25925ea91ed5fd2162_Out_2_Vector2);
    float2 _TilingAndOffset_f54c90ee27f94dda89f5cc8d9c34e46d_Out_3_Vector2;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2(1, 1), _Multiply_c94f7a1bfe8d4b25925ea91ed5fd2162_Out_2_Vector2, _TilingAndOffset_f54c90ee27f94dda89f5cc8d9c34e46d_Out_3_Vector2);
    float _Multiply_1df008c99c42489bb292da6e4b48d3e5_Out_2_Float;
    Unity_Multiply_float_float(IN.TimeParameters.x, 2, _Multiply_1df008c99c42489bb292da6e4b48d3e5_Out_2_Float);
    float _Voronoi_47fa7e61593d4b65a36df344c8445863_Out_3_Float;
    float _Voronoi_47fa7e61593d4b65a36df344c8445863_Cells_4_Float;
    Unity_Voronoi_Deterministic_float(_TilingAndOffset_f54c90ee27f94dda89f5cc8d9c34e46d_Out_3_Vector2, _Multiply_1df008c99c42489bb292da6e4b48d3e5_Out_2_Float, 2, _Voronoi_47fa7e61593d4b65a36df344c8445863_Out_3_Float, _Voronoi_47fa7e61593d4b65a36df344c8445863_Cells_4_Float);
    float _Smoothstep_e3a2badff7004345abadf6c7410be71b_Out_3_Float;
    Unity_Smoothstep_float(0.19, 1, _Voronoi_47fa7e61593d4b65a36df344c8445863_Out_3_Float, _Smoothstep_e3a2badff7004345abadf6c7410be71b_Out_3_Float);
    float _Multiply_9ec4c114033647779864bde7e5a3614e_Out_2_Float;
    Unity_Multiply_float_float(IN.TimeParameters.x, 6, _Multiply_9ec4c114033647779864bde7e5a3614e_Out_2_Float);
    float _Voronoi_871990b64fe54eee80d18a28d35e116d_Out_3_Float;
    float _Voronoi_871990b64fe54eee80d18a28d35e116d_Cells_4_Float;
    Unity_Voronoi_Deterministic_float(IN.uv0.xy, _Multiply_9ec4c114033647779864bde7e5a3614e_Out_2_Float, 3, _Voronoi_871990b64fe54eee80d18a28d35e116d_Out_3_Float, _Voronoi_871990b64fe54eee80d18a28d35e116d_Cells_4_Float);
    float _OneMinus_7bd0facf0a3a431da872d6c76d894d83_Out_1_Float;
    Unity_OneMinus_float(_Voronoi_871990b64fe54eee80d18a28d35e116d_Out_3_Float, _OneMinus_7bd0facf0a3a431da872d6c76d894d83_Out_1_Float);
    float _Multiply_961e18c2148c4431be041e997b1dc191_Out_2_Float;
    Unity_Multiply_float_float(_Smoothstep_e3a2badff7004345abadf6c7410be71b_Out_3_Float, _OneMinus_7bd0facf0a3a431da872d6c76d894d83_Out_1_Float, _Multiply_961e18c2148c4431be041e997b1dc191_Out_2_Float);
    float2 _Vector2_6131737bb6424bf48c277cc834a36caf_Out_0_Vector2 = float2(0, -0.2);
    float2 _Multiply_a2f27e64b8cb48b0863c36cfffb0235e_Out_2_Vector2;
    Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6131737bb6424bf48c277cc834a36caf_Out_0_Vector2, _Multiply_a2f27e64b8cb48b0863c36cfffb0235e_Out_2_Vector2);
    float2 _TilingAndOffset_369c76d430254125a5a23b87fa4338a5_Out_3_Vector2;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2(1, 1), _Multiply_a2f27e64b8cb48b0863c36cfffb0235e_Out_2_Vector2, _TilingAndOffset_369c76d430254125a5a23b87fa4338a5_Out_3_Vector2);
    float _SimpleNoise_0cfa228080d7497288a34880e87500cf_Out_2_Float;
    Unity_SimpleNoise_Deterministic_float(_TilingAndOffset_369c76d430254125a5a23b87fa4338a5_Out_3_Vector2, 6, _SimpleNoise_0cfa228080d7497288a34880e87500cf_Out_2_Float);
    float _Multiply_6b0e22b9f41a434184705c18ef57e86a_Out_2_Float;
    Unity_Multiply_float_float(_Multiply_961e18c2148c4431be041e997b1dc191_Out_2_Float, _SimpleNoise_0cfa228080d7497288a34880e87500cf_Out_2_Float, _Multiply_6b0e22b9f41a434184705c18ef57e86a_Out_2_Float);
    float4 _Multiply_5bccb82dfdec4280969711f59ff53f3b_Out_2_Vector4;
    Unity_Multiply_float4_float4(_Multiply_da6f4da5d7d54660901a87fbe892036a_Out_2_Vector4, (_Multiply_6b0e22b9f41a434184705c18ef57e86a_Out_2_Float.xxxx), _Multiply_5bccb82dfdec4280969711f59ff53f3b_Out_2_Vector4);
    float4 _Clamp_9371c49ceb5d411cb70df8903f3c4bfb_Out_3_Vector4;
    Unity_Clamp_float4(_Multiply_5bccb82dfdec4280969711f59ff53f3b_Out_2_Vector4, float4(0, 0, 0, 0), float4(1, 1, 1, 1), _Clamp_9371c49ceb5d411cb70df8903f3c4bfb_Out_3_Vector4);
    float4 _Add_e759ad04fa6b4b6096d55836a6d3b912_Out_2_Vector4;
    Unity_Add_float4(_Multiply_7af8452887134d268ecb368a2f91d9bc_Out_2_Vector4, _Clamp_9371c49ceb5d411cb70df8903f3c4bfb_Out_3_Vector4, _Add_e759ad04fa6b4b6096d55836a6d3b912_Out_2_Vector4);
    UnityTexture2D _Property_7ca52aaa2979442aa24e3006e200954f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
    float4 _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_7ca52aaa2979442aa24e3006e200954f_Out_0_Texture2D.tex, _Property_7ca52aaa2979442aa24e3006e200954f_Out_0_Texture2D.samplerstate, _Property_7ca52aaa2979442aa24e3006e200954f_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
    float _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_R_4_Float = _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_RGBA_0_Vector4.r;
    float _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_G_5_Float = _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_RGBA_0_Vector4.g;
    float _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_B_6_Float = _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_RGBA_0_Vector4.b;
    float _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_A_7_Float = _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_RGBA_0_Vector4.a;
    float4 _Add_73da02077750490cb044a64ee3b5cbc5_Out_2_Vector4;
    Unity_Add_float4(_Add_e759ad04fa6b4b6096d55836a6d3b912_Out_2_Vector4, _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_RGBA_0_Vector4, _Add_73da02077750490cb044a64ee3b5cbc5_Out_2_Vector4);
    float _Property_e09e852b5d864c31ba1bff2ac81c49f8_Out_0_Float = _Metallic;
    float _Property_1fec59c906d54c26912b8e1e47b76a38_Out_0_Float = _Smooth;
    float _Property_987ab245cbc641338ee3562cd775b882_Out_0_Float = _Opacity;
    surface.BaseColor = IsGammaSpace() ? float3(0.5, 0.5, 0.5) : SRGBToLinear(float3(0.5, 0.5, 0.5));
    surface.NormalTS = IN.TangentSpaceNormal;
    surface.Emission = (_Add_e759ad04fa6b4b6096d55836a6d3b912_Out_2_Vector4.xyz) * _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_A_7_Float;
    surface.Metallic = _Property_e09e852b5d864c31ba1bff2ac81c49f8_Out_0_Float;
    surface.Smoothness = _Property_1fec59c906d54c26912b8e1e47b76a38_Out_0_Float;
    surface.Occlusion = 1;
    surface.Alpha = _Property_987ab245cbc641338ee3562cd775b882_Out_0_Float * _SampleTexture2D_c146f5041a0d4dccb19e9c24d629fb46_A_7_Float;
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
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz; // we want a unit length Normal Vector node in shader graph
    output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
    output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
        
#if UNITY_UV_STARTS_AT_TOP
#else
#endif
        
        
    output.uv0 = input.texCoord0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
}