Shader "Custom/Water/DiffuseWaterOpaque_URP"
{
    Properties
    {
        _WaterColor ("Water color", Color) = (1,1,1,1)

        _WaterTex ("Water texture", 2D) = "white" {}

        _Tiling ("Water tiling", Vector) = (1,1,0,0)

        _TextureVisibility("Texture visibility", Range(0,1)) = 1

        [Space(20)]

        _DistTex ("Distortion", 2D) = "white" {}

        _DistTiling ("Distortion tiling", Vector) = (1,1,0,0)

        [Space(20)]

        _MoveDirection ("Direction", Vector) = (1,0,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float fogFactor   : TEXCOORD1;
            };

            TEXTURE2D(_WaterTex);
            SAMPLER(sampler_WaterTex);

            TEXTURE2D(_DistTex);
            SAMPLER(sampler_DistTex);

            CBUFFER_START(UnityPerMaterial)

                float4 _WaterColor;

                float2 _Tiling;
                float2 _DistTiling;

                float _TextureVisibility;

                float4 _MoveDirection;

            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;

                VertexPositionInputs posInputs =
                    GetVertexPositionInputs(v.positionOS.xyz);

                o.positionCS = posInputs.positionCS;

                o.uv = v.uv;

                o.fogFactor =
                    ComputeFogFactor(o.positionCS.z);

                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                //--------------------------------
                // UV SPACE
                //--------------------------------

                float2 water_uv = i.uv;

                //--------------------------------
                // DISTORTION
                //--------------------------------

                float4 distortion =
                    SAMPLE_TEXTURE2D(
                        _DistTex,
                        sampler_DistTex,
                        water_uv * _DistTiling
                    ) * 2 - 1;

                //--------------------------------
                // FLOW
                //--------------------------------

                float2 distorted_uv =
                    (
                        water_uv
                        + distortion.rg * 0.05
                        - _Time.y * _MoveDirection.xy
                    ) * _Tiling;

                //--------------------------------
                // WATER TEXTURE
                //--------------------------------

                float4 waterTex =
                    SAMPLE_TEXTURE2D(
                        _WaterTex,
                        sampler_WaterTex,
                        distorted_uv
                    );

                //--------------------------------
                // FINAL COLOR
                //--------------------------------

                float4 waterCol =
                    lerp(
                        _WaterColor,
                        float4(1,1,1,1),
                        waterTex.r * _TextureVisibility
                    );

                //--------------------------------
                // FOG
                //--------------------------------

                waterCol.rgb =
                    MixFog(
                        waterCol.rgb,
                        i.fogFactor
                    );

                return waterCol;
            }

            ENDHLSL
        }
    }
}