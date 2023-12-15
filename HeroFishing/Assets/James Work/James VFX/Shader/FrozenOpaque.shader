Shader"Project Hero VFX/FrozenOpaque"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        [HDR] _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        [HDR] _IceColor ("Ice Color", Color) = (1,1,1,1)
        _EdgePower ("Edge Power", Float) = 1
        [NoScaleOffset] _FreezeTex ("Freeze Texture", 2D) = "white" {}
        _FreezeMask ("Freeze Mask", Range(-0.01, 3)) = 0.5
        [Normal][NoScaleOffset] _FreezeNormalMap ("Freeze Normal", 2D) = "bump" {}
        [NoScaleOffset] _IceMask ("Ice Mask", 2D) = "white" {}
        _IceAmount ("Ice Amount", Range(0, 1)) = 0.5
        _IcicleAmount ("Icicle Amount", Range(0,2)) = 1
        _IcicleScale ("Icicle Scale", Float) = .5
        _FreezeRandom ("Freeze Random", Vector) = (0.5,0.5,0,0)
    }
    SubShader
    {
        Tags {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
            "UniversalMaterialType"="Lit"}
        LOD 100

        Pass
        {
            Tags {
                "LightMode" = "UniversalForward"
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                float4 vertex : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_IceMask);
            SAMPLER(sampler_IceMask);
            TEXTURE2D(_FreezeTex);
            SAMPLER(sampler_FreezeTex);
            TEXTURE2D(_FreezeNormalMap);
            SAMPLER(sampler_FreezeNormalMap);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _IceMask_ST;
            half _IcicleAmount;
            half2 _FreezeRandom;
            half _IcicleScale;
            half _EdgePower;
            half4 _EdgeColor;
            half4 _IceColor;
            half _IceAmount;
            float4 _FreezeNormalMap_ST;
            half _FreezeMask;
            float4 _FreezeTex_ST;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                // 取得世界座標的法線
                float3 worldNormal = TransformObjectToWorldNormal(v.normal);
                // 取得第一UV
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // uv加入偏移量
                o.uv2 = o.uv + _FreezeRandom;
                // 取得遮罩數值
                half4 maskCol = SAMPLE_TEXTURE2D_LOD(_IceMask, sampler_IceMask, half4(o.uv2.xy, 0, 0), 0);
                // 將頂點根據冰凍程度，遮罩數值及朝上的法線去做偏移
                o.vertex = TransformObjectToHClip(pow(saturate(worldNormal.y * _IcicleAmount * maskCol), _IcicleScale) * v.normal + v.vertex);
                // 設定世界位置及世界法線
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = worldNormal;
                return o;
            }
            

            float2 unity_gradientNoise_dir(float2 p){
                p = p % 289;
                float x = (34 * p.x + 1) + p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float unity_gradientNoise(float2 p) {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(unity_gradientNoise_dir(ip), fp);
                float d01 = dot(unity_gradientNoise_dir(ip + float2(0,1)), fp - float2(0,1));
                float d10 = dot(unity_gradientNoise_dir(ip + float2(1,0)), fp - float2(1,0));
                float d11 = dot(unity_gradientNoise_dir(ip + float2(1,1)), fp - float2(1,1));
                fp = fp * fp * fp * (fp * (fp * 6 -15) + 10);
                return lerp(lerp(d00,d01,fp.y), lerp(d10,d11,fp.y), fp.x);
            }

            void Unity_GradientNoise_float(float2 UV, float scale, out float output) {
                output = unity_gradientNoise(UV * scale) + 0.5;
            }

            half4 frag (v2f i) : SV_Target
            {
                // 取得光源方向
                half3 lightDir = _MainLightPosition.xyz - i.worldPos;
                // 取得視線方向
                half3 viewDir = normalize(GetWorldSpaceViewDir(i.worldPos));
                // 取得冰凍的法向量貼圖的值
                half3 mapNormal = UnpackNormal(SAMPLE_TEXTURE2D(_FreezeNormalMap, sampler_FreezeNormalMap, i.uv));   
                // 法向量根據冰凍程度接近冰凍貼圖或者原本的法向量
                half3 worldNormal = lerp(i.worldNormal, mapNormal, _IceAmount);
                // 取得反射向量
                half3 worldRefl = reflect(-viewDir, worldNormal);
                // 取得天空球的顏色
                half3 sh = SampleSH(worldRefl);
                // 計算diffuse
                half3 diffuse = (saturate(dot(worldNormal, lightDir)));
                // 計算受光的顏色
                half3 lightColor = (diffuse + sh) * _MainLightColor;   
                // 取得物體邊緣，相當於Fresnel
                half rim = pow(1.0 - saturate(dot(viewDir, i.worldNormal)), _EdgePower);                

                // 生成Gradient Noise貼圖，一種根據UV的Noise貼圖
                half output;
                Unity_GradientNoise_float(i.uv2, 5.72, output);
                output = smoothstep(1 - _FreezeMask, 1.01, saturate(output));
                // 取得原本的貼圖顏色跟冰凍的貼圖顏色
                half4 mainCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 freezeCol = SAMPLE_TEXTURE2D(_FreezeTex, sampler_FreezeTex, i.uv) * _IceColor;
                // 混合兩者貼圖，根據Noise的遮罩跟冰凍程度去判斷其冰凍的深淺。
                half3 texCol = lerp(mainCol.rgb, freezeCol.rgb, output * _IceAmount);
                // 貼圖顏色與光線顏色相乘，並且加入邊緣光
                half3 color = lerp(texCol * lightColor, _EdgeColor, rim);
                //half3 color = mainCol * lightColor;
   
                return half4(color, mainCol.a);
            }
            ENDHLSL
        }
    }
}
