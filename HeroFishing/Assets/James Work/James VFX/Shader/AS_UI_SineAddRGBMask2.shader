// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AS_UI_SineAddRGBMask2"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[ASEBegin]_MainTex("MainTex", 2D) = "white" {}
		_MainTexture("MainTexture", 2D) = "white" {}
		_AddMaskTexture("Add Mask Texture", 2D) = "white" {}
		[HDR]_ColorR("Color-R", Color) = (1,1,1,0)
		[HDR]_ColorG("Color-G", Color) = (1,1,1,0)
		[HDR]_ColorB("Color-B", Color) = (1,1,1,0)
		_LightOffestR("Light Offest-R", Float) = 0
		_LightOffestG("Light Offest-G", Float) = 0.3
		_LightOffestB("Light Offest-B", Float) = 0.5
		_SinLightArea("Sin Light Area", Range( 0 , 1)) = 0.5
		_SineLigjtSpeed("Sine Ligjt Speed", Float) = 0
		[Toggle(_USEOFFSETINSTEADOFTIME_ON)] _UseOffsetInsteadofTime("Use Offset Instead of Time", Float) = 0
		[ASEEnd]_SineLightOffset("Sine Light Offset", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}


		[Header(UGUI Stencil)]
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 
		ENDHLSL

		/* UGUI */
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}

		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Sprite Lit"
			Tags { "LightMode"="Universal2D" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 100400


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_0
			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_1
			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_2
			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_3

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS_SPRITELIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
			
			#if USE_SHAPE_LIGHT_TYPE_0
			SHAPE_LIGHT(0)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_1
			SHAPE_LIGHT(1)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_2
			SHAPE_LIGHT(2)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_3
			SHAPE_LIGHT(3)
			#endif

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

			#pragma shader_feature_local _USEOFFSETINSTEADOFTIME_ON


			sampler2D _MainTexture;
			sampler2D _AddMaskTexture;
			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _MainTexture_ST;
			float4 _AddMaskTexture_ST;
			float4 _ColorR;
			float4 _ColorG;
			float4 _ColorB;
			float4 _MainTex_ST;
			float _SinLightArea;
			float _SineLigjtSpeed;
			float _SineLightOffset;
			float _LightOffestR;
			float _LightOffestG;
			float _LightOffestB;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float4 screenPosition : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D(_AlphaTex); SAMPLER(sampler_AlphaTex);
				float _EnableAlphaTexture;
			#endif

			
			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;
				o.screenPosition = ComputeScreenPos( o.clipPos, _ProjectionParams.x );
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_MainTexture = IN.texCoord0.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
				float4 tex2DNode1 = tex2D( _MainTexture, uv_MainTexture );
				float2 uv_AddMaskTexture = IN.texCoord0.xy * _AddMaskTexture_ST.xy + _AddMaskTexture_ST.zw;
				float temp_output_52_0 = (-1.0 + (_SinLightArea - 0.0) * (2.0 - -1.0) / (1.0 - 0.0));
				float2 temp_cast_0 = (1.0).xx;
				float mulTime31 = _TimeParameters.x * _SineLigjtSpeed;
				#ifdef _USEOFFSETINSTEADOFTIME_ON
				float staticSwitch86 = _SineLightOffset;
				#else
				float staticSwitch86 = mulTime31;
				#endif
				float2 temp_cast_1 = (( staticSwitch86 + _LightOffestR )).xx;
				float2 texCoord48 = IN.texCoord0.xy * temp_cast_0 + temp_cast_1;
				float cos47 = cos( 100.0 );
				float sin47 = sin( 100.0 );
				float2 rotator47 = mul( texCoord48 - float2( 0.5,0.5 ) , float2x2( cos47 , -sin47 , sin47 , cos47 )) + float2( 0.5,0.5 );
				float clampResult32 = clamp( ( temp_output_52_0 + sin( (rotator47).x ) ) , 0.0 , 1.0 );
				float2 temp_cast_2 = (1.0).xx;
				float2 temp_cast_3 = (( staticSwitch86 + _LightOffestG )).xx;
				float2 texCoord55 = IN.texCoord0.xy * temp_cast_2 + temp_cast_3;
				float cos64 = cos( 100.0 );
				float sin64 = sin( 100.0 );
				float2 rotator64 = mul( texCoord55 - float2( 0.5,0.5 ) , float2x2( cos64 , -sin64 , sin64 , cos64 )) + float2( 0.5,0.5 );
				float clampResult71 = clamp( ( temp_output_52_0 + sin( (rotator64).x ) ) , 0.0 , 1.0 );
				float2 temp_cast_4 = (1.0).xx;
				float2 temp_cast_5 = (( staticSwitch86 + _LightOffestB )).xx;
				float2 texCoord63 = IN.texCoord0.xy * temp_cast_4 + temp_cast_5;
				float cos65 = cos( 100.0 );
				float sin65 = sin( 100.0 );
				float2 rotator65 = mul( texCoord63 - float2( 0.5,0.5 ) , float2x2( cos65 , -sin65 , sin65 , cos65 )) + float2( 0.5,0.5 );
				float clampResult72 = clamp( ( temp_output_52_0 + sin( (rotator65).x ) ) , 0.0 , 1.0 );
				float2 uv_MainTex = IN.texCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				
				float4 Color = ( tex2DNode1 + ( tex2DNode1.a * ( ( tex2D( _AddMaskTexture, uv_AddMaskTexture ).r * clampResult32 * _ColorR ) + ( tex2D( _AddMaskTexture, uv_AddMaskTexture ).g * clampResult71 * _ColorG ) + ( tex2D( _AddMaskTexture, uv_AddMaskTexture ).b * clampResult72 * _ColorB ) ) ) + ( tex2D( _MainTex, uv_MainTex ) * float4( 0,0,0,0 ) ) );
				float Mask = tex2DNode1.a;
				float3 Normal = float3( 0, 0, 1 );

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, IN.texCoord0.xy);
					Color.a = lerp ( Color.a, alpha.r, _EnableAlphaTexture);
				#endif
				
				Color *= IN.color;

				return CombinedShapeLightShared( Color, Mask, IN.screenPosition.xy / IN.screenPosition.w );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "Sprite Normal"
			Tags { "LightMode"="NormalsRendering" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 100400


			#pragma vertex vert
			#pragma fragment frag

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS_SPRITENORMAL

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"
			
			#pragma shader_feature_local _USEOFFSETINSTEADOFTIME_ON


			sampler2D _MainTexture;
			sampler2D _AddMaskTexture;
			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _MainTexture_ST;
			float4 _AddMaskTexture_ST;
			float4 _ColorR;
			float4 _ColorG;
			float4 _ColorB;
			float4 _MainTex_ST;
			float _SinLightArea;
			float _SineLigjtSpeed;
			float _SineLightOffset;
			float _LightOffestR;
			float _LightOffestG;
			float _LightOffestB;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float3 normalWS : TEXCOORD2;
				float4 tangentWS : TEXCOORD3;
				float3 bitangentWS : TEXCOORD4;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;

				float3 normalWS = TransformObjectToWorldNormal( v.normal );
				o.normalWS = NormalizeNormalPerVertex( normalWS );
				float4 tangentWS = float4( TransformObjectToWorldDir( v.tangent.xyz ), v.tangent.w );
				o.tangentWS = normalize( tangentWS );
				o.bitangentWS = cross( normalWS, tangentWS.xyz ) * tangentWS.w;
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_MainTexture = IN.texCoord0.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
				float4 tex2DNode1 = tex2D( _MainTexture, uv_MainTexture );
				float2 uv_AddMaskTexture = IN.texCoord0.xy * _AddMaskTexture_ST.xy + _AddMaskTexture_ST.zw;
				float temp_output_52_0 = (-1.0 + (_SinLightArea - 0.0) * (2.0 - -1.0) / (1.0 - 0.0));
				float2 temp_cast_0 = (1.0).xx;
				float mulTime31 = _TimeParameters.x * _SineLigjtSpeed;
				#ifdef _USEOFFSETINSTEADOFTIME_ON
				float staticSwitch86 = _SineLightOffset;
				#else
				float staticSwitch86 = mulTime31;
				#endif
				float2 temp_cast_1 = (( staticSwitch86 + _LightOffestR )).xx;
				float2 texCoord48 = IN.texCoord0.xy * temp_cast_0 + temp_cast_1;
				float cos47 = cos( 100.0 );
				float sin47 = sin( 100.0 );
				float2 rotator47 = mul( texCoord48 - float2( 0.5,0.5 ) , float2x2( cos47 , -sin47 , sin47 , cos47 )) + float2( 0.5,0.5 );
				float clampResult32 = clamp( ( temp_output_52_0 + sin( (rotator47).x ) ) , 0.0 , 1.0 );
				float2 temp_cast_2 = (1.0).xx;
				float2 temp_cast_3 = (( staticSwitch86 + _LightOffestG )).xx;
				float2 texCoord55 = IN.texCoord0.xy * temp_cast_2 + temp_cast_3;
				float cos64 = cos( 100.0 );
				float sin64 = sin( 100.0 );
				float2 rotator64 = mul( texCoord55 - float2( 0.5,0.5 ) , float2x2( cos64 , -sin64 , sin64 , cos64 )) + float2( 0.5,0.5 );
				float clampResult71 = clamp( ( temp_output_52_0 + sin( (rotator64).x ) ) , 0.0 , 1.0 );
				float2 temp_cast_4 = (1.0).xx;
				float2 temp_cast_5 = (( staticSwitch86 + _LightOffestB )).xx;
				float2 texCoord63 = IN.texCoord0.xy * temp_cast_4 + temp_cast_5;
				float cos65 = cos( 100.0 );
				float sin65 = sin( 100.0 );
				float2 rotator65 = mul( texCoord63 - float2( 0.5,0.5 ) , float2x2( cos65 , -sin65 , sin65 , cos65 )) + float2( 0.5,0.5 );
				float clampResult72 = clamp( ( temp_output_52_0 + sin( (rotator65).x ) ) , 0.0 , 1.0 );
				float2 uv_MainTex = IN.texCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				
				float4 Color = ( tex2DNode1 + ( tex2DNode1.a * ( ( tex2D( _AddMaskTexture, uv_AddMaskTexture ).r * clampResult32 * _ColorR ) + ( tex2D( _AddMaskTexture, uv_AddMaskTexture ).g * clampResult71 * _ColorG ) + ( tex2D( _AddMaskTexture, uv_AddMaskTexture ).b * clampResult72 * _ColorB ) ) ) + ( tex2D( _MainTex, uv_MainTex ) * float4( 0,0,0,0 ) ) );
				float3 Normal = float3( 0, 0, 1 );
				
				Color *= IN.color;

				return NormalsRenderingShared( Color, Normal, IN.tangentWS.xyz, IN.bitangentWS, IN.normalWS);
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "Sprite Forward"
			Tags { "LightMode"="UniversalForward" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 100400


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS_SPRITEFORWARD

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#pragma shader_feature_local _USEOFFSETINSTEADOFTIME_ON


			sampler2D _MainTexture;
			sampler2D _AddMaskTexture;
			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _MainTexture_ST;
			float4 _AddMaskTexture_ST;
			float4 _ColorR;
			float4 _ColorG;
			float4 _ColorB;
			float4 _MainTex_ST;
			float _SinLightArea;
			float _SineLigjtSpeed;
			float _SineLightOffset;
			float _LightOffestR;
			float _LightOffestG;
			float _LightOffestB;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.vertex.xyz );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_MainTexture = IN.texCoord0.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
				float4 tex2DNode1 = tex2D( _MainTexture, uv_MainTexture );
				float2 uv_AddMaskTexture = IN.texCoord0.xy * _AddMaskTexture_ST.xy + _AddMaskTexture_ST.zw;
				float temp_output_52_0 = (-1.0 + (_SinLightArea - 0.0) * (2.0 - -1.0) / (1.0 - 0.0));
				float2 temp_cast_0 = (1.0).xx;
				float mulTime31 = _TimeParameters.x * _SineLigjtSpeed;
				#ifdef _USEOFFSETINSTEADOFTIME_ON
				float staticSwitch86 = _SineLightOffset;
				#else
				float staticSwitch86 = mulTime31;
				#endif
				float2 temp_cast_1 = (( staticSwitch86 + _LightOffestR )).xx;
				float2 texCoord48 = IN.texCoord0.xy * temp_cast_0 + temp_cast_1;
				float cos47 = cos( 100.0 );
				float sin47 = sin( 100.0 );
				float2 rotator47 = mul( texCoord48 - float2( 0.5,0.5 ) , float2x2( cos47 , -sin47 , sin47 , cos47 )) + float2( 0.5,0.5 );
				float clampResult32 = clamp( ( temp_output_52_0 + sin( (rotator47).x ) ) , 0.0 , 1.0 );
				float2 temp_cast_2 = (1.0).xx;
				float2 temp_cast_3 = (( staticSwitch86 + _LightOffestG )).xx;
				float2 texCoord55 = IN.texCoord0.xy * temp_cast_2 + temp_cast_3;
				float cos64 = cos( 100.0 );
				float sin64 = sin( 100.0 );
				float2 rotator64 = mul( texCoord55 - float2( 0.5,0.5 ) , float2x2( cos64 , -sin64 , sin64 , cos64 )) + float2( 0.5,0.5 );
				float clampResult71 = clamp( ( temp_output_52_0 + sin( (rotator64).x ) ) , 0.0 , 1.0 );
				float2 temp_cast_4 = (1.0).xx;
				float2 temp_cast_5 = (( staticSwitch86 + _LightOffestB )).xx;
				float2 texCoord63 = IN.texCoord0.xy * temp_cast_4 + temp_cast_5;
				float cos65 = cos( 100.0 );
				float sin65 = sin( 100.0 );
				float2 rotator65 = mul( texCoord63 - float2( 0.5,0.5 ) , float2x2( cos65 , -sin65 , sin65 , cos65 )) + float2( 0.5,0.5 );
				float clampResult72 = clamp( ( temp_output_52_0 + sin( (rotator65).x ) ) , 0.0 , 1.0 );
				float2 uv_MainTex = IN.texCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				
				float4 Color = ( tex2DNode1 + ( tex2DNode1.a * ( ( tex2D( _AddMaskTexture, uv_AddMaskTexture ).r * clampResult32 * _ColorR ) + ( tex2D( _AddMaskTexture, uv_AddMaskTexture ).g * clampResult71 * _ColorG ) + ( tex2D( _AddMaskTexture, uv_AddMaskTexture ).b * clampResult72 * _ColorB ) ) ) + ( tex2D( _MainTex, uv_MainTex ) * float4( 0,0,0,0 ) ) );

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif

				Color *= IN.color;

				return Color;
			}

			ENDHLSL
		}
		
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18912
1920;0;1920;1019;2833.079;-1359.825;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;53;-2281.777,1674.121;Inherit;False;Property;_SineLigjtSpeed;Sine Ligjt Speed;10;0;Create;True;0;0;0;False;0;False;0;-11;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;31;-2104.501,1676.546;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-2124.29,1761.033;Inherit;False;Property;_SineLightOffset;Sine Light Offset;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-1645.588,1522.796;Inherit;False;Property;_LightOffestR;Light Offest-R;6;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1615.226,1881.198;Inherit;False;Property;_LightOffestG;Light Offest-G;7;0;Create;True;0;0;0;False;0;False;0.3;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-1603.861,2164.644;Inherit;False;Property;_LightOffestB;Light Offest-B;8;0;Create;True;0;0;0;False;0;False;0.5;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;86;-1888.29,1616.033;Inherit;False;Property;_UseOffsetInsteadofTime;Use Offset Instead of Time;11;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-1897.19,1871.04;Inherit;False;Constant;_SineTiling;Sine Tiling;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;62;-1556.819,2069.813;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-1564.108,1416.001;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-1566.226,1785.198;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;63;-1411.903,2029.475;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;48;-1416.215,1408.235;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;55;-1420.31,1743.86;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;41;-1376.592,1617.754;Inherit;False;Constant;_LightRotation;Light Rotation;13;0;Create;True;0;0;0;False;0;False;100;60;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;64;-1129.226,1670.198;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;65;-1138.666,2020.431;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;47;-1120.592,1287.754;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;67;-950.1448,2025.851;Inherit;False;True;False;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;51;-945.7583,1290.828;Inherit;False;True;False;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;66;-948.5283,1671.89;Inherit;False;True;False;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-1121.399,1423.944;Inherit;False;Property;_SinLightArea;Sin Light Area;9;0;Create;True;0;0;0;False;0;False;0.5;0.02;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;68;-749.4479,2027.353;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;52;-913.6661,1502.083;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;54;-745.6637,1673.957;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;50;-747.8893,1292.349;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-526.8624,1303.682;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;70;-539.0561,2022.37;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;69;-523.951,1665.445;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;17;-1299.492,784.0112;Inherit;True;Property;_AddMaskTexture;Add Mask Texture;2;0;Create;True;0;0;0;False;0;False;None;da2aee3f9a34a724fb38dc7afeee794a;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;22;-701.5131,671.5217;Inherit;True;Property;_TextureSample2;Texture Sample 2;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;82;-444.8927,1810.781;Inherit;False;Property;_ColorG;Color-G;4;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;4.391945,3.674434,5.379697,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;74;-700.0723,1075.262;Inherit;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;83;-439.2253,2159.413;Inherit;False;Property;_ColorB;Color-B;5;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;4,4,4,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;71;-383.8139,1665.161;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;32;-384.8888,1307.398;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;25;-451.6765,1439.869;Inherit;False;Property;_ColorR;Color-R;3;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;12.68145,11.5211,8.900942,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;-699.542,874.4864;Inherit;True;Property;_TextureSample3;Texture Sample 3;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;72;-381.1578,2027.084;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-151.5619,2025.14;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-154.211,1666.411;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-154.5659,1314.277;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;687.6102,872.5197;Inherit;True;Property;_MainTexture;MainTexture;1;0;Create;True;0;0;0;False;0;False;-1;None;f914119c47ac92b4aaa10aec7b68056c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;80;211.5093,1499.559;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;27;608.1292,1694.144;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;701.3761,1388.693;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;1012.66,1735.517;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;1047.747,1313.793;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;81;459.9321,1636.642;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;101;1438.191,1328.285;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;12;AS_UI_SineAddRGBMask2;199187dac283dbe4a8cb1ea611d70c58;True;Sprite Lit;0;0;Sprite Lit;6;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-7;False;False;False;False;False;False;False;True;True;0;True;-3;255;True;-6;255;True;-5;0;True;-2;0;True;-4;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;2;5;False;-1;10;False;-1;3;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;1;Vertex Position;1;0;3;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;103;1438.191,1328.285;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;12;New Amplify Shader;199187dac283dbe4a8cb1ea611d70c58;True;Sprite Forward;0;2;Sprite Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;True;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;2;5;False;-1;10;False;-1;3;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;102;1438.191,1328.285;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;12;New Amplify Shader;199187dac283dbe4a8cb1ea611d70c58;True;Sprite Normal;0;1;Sprite Normal;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-7;False;False;False;False;False;False;False;True;True;0;True;-3;255;True;-6;255;True;-5;0;True;-2;0;True;-4;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;2;5;False;-1;10;False;-1;3;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=NormalsRendering;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;31;0;53;0
WireConnection;86;1;31;0
WireConnection;86;0;85;0
WireConnection;62;0;86;0
WireConnection;62;1;60;0
WireConnection;61;0;86;0
WireConnection;61;1;58;0
WireConnection;57;0;86;0
WireConnection;57;1;59;0
WireConnection;63;0;84;0
WireConnection;63;1;62;0
WireConnection;48;0;84;0
WireConnection;48;1;61;0
WireConnection;55;0;84;0
WireConnection;55;1;57;0
WireConnection;64;0;55;0
WireConnection;64;2;41;0
WireConnection;65;0;63;0
WireConnection;65;2;41;0
WireConnection;47;0;48;0
WireConnection;47;2;41;0
WireConnection;67;0;65;0
WireConnection;51;0;47;0
WireConnection;66;0;64;0
WireConnection;68;0;67;0
WireConnection;52;0;33;0
WireConnection;54;0;66;0
WireConnection;50;0;51;0
WireConnection;34;0;52;0
WireConnection;34;1;50;0
WireConnection;70;0;52;0
WireConnection;70;1;68;0
WireConnection;69;0;52;0
WireConnection;69;1;54;0
WireConnection;22;0;17;0
WireConnection;74;0;17;0
WireConnection;71;0;69;0
WireConnection;32;0;34;0
WireConnection;23;0;17;0
WireConnection;72;0;70;0
WireConnection;79;0;74;3
WireConnection;79;1;72;0
WireConnection;79;2;83;0
WireConnection;78;0;23;2
WireConnection;78;1;71;0
WireConnection;78;2;82;0
WireConnection;77;0;22;1
WireConnection;77;1;32;0
WireConnection;77;2;25;0
WireConnection;80;0;77;0
WireConnection;80;1;78;0
WireConnection;80;2;79;0
WireConnection;3;0;1;4
WireConnection;3;1;80;0
WireConnection;87;0;27;0
WireConnection;5;0;1;0
WireConnection;5;1;3;0
WireConnection;5;2;87;0
WireConnection;101;1;5;0
WireConnection;101;2;1;4
ASEEND*/
//CHKSM=C3FFEF348461CC4C34B0F9495E791B3D840BDAA1