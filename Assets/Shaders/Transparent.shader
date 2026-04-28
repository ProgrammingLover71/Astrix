Shader "Custom/Transparent"
{
    Properties
    {
        _Opacity("Opacity", Range(0, 1)) = 0.2
        _Color("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderQueue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex   Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            //------ Inputs ------//
            
            float _Opacity;
            float4 _Color;

            //------ Vertex Shader ------//

            Varyings Vertex(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            //------ Fragment Shader ------//

            float4 Fragment(Varyings IN) : SV_Target
            {
                float4 color = float4(_Color.xyz, _Opacity);
                return color;
            }
            ENDHLSL
        }
    }
}
