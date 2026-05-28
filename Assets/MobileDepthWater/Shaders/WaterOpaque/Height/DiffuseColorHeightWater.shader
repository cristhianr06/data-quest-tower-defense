Shader "Custom/Water/Height/DiffuseColorWater_URP"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)

        [Space(20)]
        _WaterColor ("Water color", Color) = (1,1,1,1)
        _WaterTex("Water texture", 2D) = "white" {}
        _Tiling ("Water tiling", Vector) = (1,1,0,0)
        _TextureVisibility("Texture visibility", Range(0,1)) = 1

        [Space(20)]
        _DistTex ("Distortion", 2D) = "white" {}
        _DistTiling ("Distortion tiling", Vector) = (1,1,0,0)

        [Space(20)]
        _WaterHeight ("Water height", Float) = 0
        _WaterDeep ("Water deep", Float) = 0
        _WaterMinAlpha ("Water min alpha", Range(0,1)) = 0

        [Space(20)]
        _BorderColor ("Border color", Color) = (1,1,1,1)
        _BorderWidth ("Border width", Range(0,1)) = 0

        [Space(20)]
        _MoveDirection ("Direction", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "Forward"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;

            #ifdef LIGHTMAP_ON
                float2 lightmapUV : TEXCOORD1;
            #endif
            };

            struct Varyings
            {
                float4 positionCS         : SV_POSITION;
                float3 worldPos           : TEXCOORD0;
                float2 uv                 : TEXCOORD1;
                float  camHeightOverWater : TEXCOORD2;
                float  fogFactor          : TEXCOORD3;

            #ifdef LIGHTMAP_ON
                float2 lightmapUV         : TEXCOORD4;
            #else
                float3 diffuseLight       : TEXCOORD4;
            #endif
            };

            TEXTURE2D(_WaterTex);
            SAMPLER(sampler_WaterTex);

            TEXTURE2D(_DistTex);
            SAMPLER(sampler_DistTex);

            CBUFFER_START(UnityPerMaterial)

                float4 _Color;

                float4 _WaterColor;

                float2 _Tiling;
                float2 _DistTiling;

                float _TextureVisibility;

                float _WaterHeight;
                float _WaterDeep;
                float _WaterMinAlpha;

                float4 _BorderColor;
                float _BorderWidth;

                float4 _MoveDirection;

            CBUFFER_END

            float3 DiffuseLight(float3 normalWS)
            {
                Light mainLight = GetMainLight();

                float NdotL = saturate(dot(normalWS, mainLight.direction));

                float3 diffuse =
                    mainLight.color * NdotL;

                diffuse += SampleSH(normalWS);

                return diffuse;
            }

            float2 WaterPlaneUV(float3 worldPos, float camHeightOverWater)
            {
                float3 camToWorldRay =
                    worldPos - _WorldSpaceCameraPos;

                float3 rayToWaterPlane =
                    (camHeightOverWater / camToWorldRay.y)
                    * camToWorldRay;

                return rayToWaterPlane.xz
                    - _WorldSpaceCameraPos.xz;
            }

            float3 MainColor(Varyings i)
            {
                float3 col = _Color.rgb;

            #ifdef LIGHTMAP_ON

                col *= SampleLightmap(
                    i.lightmapUV,
                    0,
                    float3(0,0,0)
                );

            #else

                col *= i.diffuseLight;

            #endif

                return col;
            }

            Varyings vert(Attributes v)
            {
                Varyings o;

                VertexPositionInputs posInputs =
                    GetVertexPositionInputs(v.positionOS.xyz);

                VertexNormalInputs normalInputs =
                    GetVertexNormalInputs(v.normalOS);

                o.positionCS = posInputs.positionCS;
                o.worldPos = posInputs.positionWS;

                o.uv = v.uv;

                o.camHeightOverWater =
                    _WorldSpaceCameraPos.y - _WaterHeight;

                o.fogFactor =
                    ComputeFogFactor(o.positionCS.z);

            #ifdef LIGHTMAP_ON

                o.lightmapUV =
                    v.lightmapUV * unity_LightmapST.xy
                    + unity_LightmapST.zw;

            #else

                o.diffuseLight =
                    DiffuseLight(normalInputs.normalWS);

            #endif

                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float lengthUnderWater =
                    max(0, _WaterHeight - i.worldPos.y);

                float underWater =
                    step(0.0001, lengthUnderWater);

                float borderAlpha =
                    lerp(
                        underWater * _BorderColor.a,
                        0,
                        saturate(lengthUnderWater / _BorderWidth)
                    );

                float waterAlpha =
                    saturate(
                        lengthUnderWater / _WaterDeep
                        + _WaterMinAlpha
                    );

                float3 mainCol =
                    MainColor(i);

                float2 water_uv =
                    WaterPlaneUV(
                        i.worldPos,
                        i.camHeightOverWater
                    );

                float4 distortion =
                    SAMPLE_TEXTURE2D(
                        _DistTex,
                        sampler_DistTex,
                        water_uv * _DistTiling
                    ) * 2 - 1;

                float2 distorted_uv =
                    (
                        (water_uv + distortion.rg)
                        - _Time.y * _MoveDirection.xz
                    ) * _Tiling;

                float4 waterTex =
                    SAMPLE_TEXTURE2D(
                        _WaterTex,
                        sampler_WaterTex,
                        distorted_uv
                    );

                float4 waterCol =
                    lerp(
                        _WaterColor,
                        float4(1,1,1,1),
                        waterTex.r * _TextureVisibility
                    );

                float4 finalCol =
                    lerp(
                        float4(mainCol, 1),
                        waterCol,
                        _WaterColor.a
                        * waterAlpha
                        * underWater
                    );

                finalCol.rgb =
                    lerp(
                        finalCol.rgb,
                        _BorderColor.rgb,
                        borderAlpha
                    );

                finalCol.rgb =
                    MixFog(finalCol.rgb, i.fogFactor);

                return finalCol;
            }

            ENDHLSL
        }
    }
}