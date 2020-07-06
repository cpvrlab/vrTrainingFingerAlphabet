Shader "CloudGenerator"
{
    Properties
    {
        Vector4_F9BF86A4("RotateProjection", Vector) = (1, 0, 0, 90)
        Vector1_37F2A07E("NoiseScale", Float) = 10
        Vector1_62892E34("NoiseSpeed", Float) = 0.1
        Vector1_C7F9BC69("noiseHeight", Float) = 1
        Vector4_B6649628("NoiseRemap", Vector) = (0, 1, -1, 1)
        Color_982234E("Color Peak", Color) = (1, 1, 1, 0)
        Color_4089CE7A("Color Valley", Color) = (0, 0, 0, 0)
        Vector1_8E963D5("NoiseEdge1", Float) = 0
        Vector1_AECCD54A("NoiseEdge2", Float) = 1
        Vector1_F0BE3006("Power", Float) = 1
        Vector1_6BAC9A68("BaseScale", Float) = 5
        Vector1_5ED40136("BaseSpeed", Float) = 0.1
        Vector1_DFA4C9D3("BaseStrength", Float) = 2
        Vector1_C4072198("CurvertureRadius", Float) = 1
        Vector1_F3A863FF("FrenselPower", Float) = 0.5
        Vector1_4074C868("Frensel Opacity", Float) = 1
        Vector1_ED18166C("FadeDepth", Float) = 1000
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent+0"
        }
        
        Pass
        {
            Name "Pass"
            Tags 
            { 
                // LightMode: <None>
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Off
            ZTest LEqual
            ZWrite On
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma shader_feature _ _SAMPLE_GI
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS 
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS_UNLIT
            #define REQUIRE_DEPTH_TEXTURE
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 Vector4_F9BF86A4;
            float Vector1_37F2A07E;
            float Vector1_62892E34;
            float Vector1_C7F9BC69;
            float4 Vector4_B6649628;
            float4 Color_982234E;
            float4 Color_4089CE7A;
            float Vector1_8E963D5;
            float Vector1_AECCD54A;
            float Vector1_F0BE3006;
            float Vector1_6BAC9A68;
            float Vector1_5ED40136;
            float Vector1_DFA4C9D3;
            float Vector1_C4072198;
            float Vector1_F3A863FF;
            float Vector1_4074C868;
            float Vector1_ED18166C;
            CBUFFER_END
        
            // Graph Functions
            
            void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
            {
                Rotation = radians(Rotation);
            
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
                
                Axis = normalize(Axis);
            
                float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                          one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                          one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                        };
            
                Out = mul(rot_mat,  In);
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
            
            
            float2 Unity_GradientNoise_Dir_float(float2 p)
            {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            
            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            { 
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
                float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }
            
            void Unity_Power_float(float A, float B, out float Out)
            {
                Out = pow(A, B);
            }
            
            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }
            
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }
            
            void Unity_Absolute_float(float In, out float Out)
            {
                Out = abs(In);
            }
            
            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
            
            void Unity_Add_float3(float3 A, float3 B, out float3 Out)
            {
                Out = A + B;
            }
            
            void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
            {
                Out = lerp(A, B, T);
            }
            
            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }
            
            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }
            
            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
            
            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }
        
            // Graph Vertex
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
                float3 AbsoluteWorldSpacePosition;
                float3 TimeParameters;
            };
            
            struct VertexDescription
            {
                float3 VertexPosition;
                float3 VertexNormal;
                float3 VertexTangent;
            };
            
            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float _Property_31BA0AD9_Out_0 = Vector1_8E963D5;
                float _Property_3C5B259_Out_0 = Vector1_AECCD54A;
                float4 _Property_5632646D_Out_0 = Vector4_F9BF86A4;
                float _Split_55D0BCE5_R_1 = _Property_5632646D_Out_0[0];
                float _Split_55D0BCE5_G_2 = _Property_5632646D_Out_0[1];
                float _Split_55D0BCE5_B_3 = _Property_5632646D_Out_0[2];
                float _Split_55D0BCE5_A_4 = _Property_5632646D_Out_0[3];
                float3 _RotateAboutAxis_D26A2BC7_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.AbsoluteWorldSpacePosition, (_Property_5632646D_Out_0.xyz), _Split_55D0BCE5_A_4, _RotateAboutAxis_D26A2BC7_Out_3);
                float _Property_81426031_Out_0 = Vector1_62892E34;
                float _Multiply_F54EAFD8_Out_2;
                Unity_Multiply_float(_Property_81426031_Out_0, IN.TimeParameters.x, _Multiply_F54EAFD8_Out_2);
                float2 _TilingAndOffset_166FCC6_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_F54EAFD8_Out_2.xx), _TilingAndOffset_166FCC6_Out_3);
                float _Property_1556047C_Out_0 = Vector1_37F2A07E;
                float _GradientNoise_BA765EAA_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_166FCC6_Out_3, _Property_1556047C_Out_0, _GradientNoise_BA765EAA_Out_2);
                float2 _TilingAndOffset_F368CF6E_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_F54EAFD8_Out_2.xx), _TilingAndOffset_F368CF6E_Out_3);
                float _GradientNoise_578ADE08_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_F368CF6E_Out_3, _Property_1556047C_Out_0, _GradientNoise_578ADE08_Out_2);
                float _Add_EC2BDC4A_Out_2;
                Unity_Add_float(_GradientNoise_BA765EAA_Out_2, _GradientNoise_578ADE08_Out_2, _Add_EC2BDC4A_Out_2);
                float _Divide_24FB0AD0_Out_2;
                Unity_Divide_float(_Add_EC2BDC4A_Out_2, 2, _Divide_24FB0AD0_Out_2);
                float _Saturate_6E0633DE_Out_1;
                Unity_Saturate_float(_Divide_24FB0AD0_Out_2, _Saturate_6E0633DE_Out_1);
                float _Property_C3F02F9F_Out_0 = Vector1_F0BE3006;
                float _Power_EA222828_Out_2;
                Unity_Power_float(_Saturate_6E0633DE_Out_1, _Property_C3F02F9F_Out_0, _Power_EA222828_Out_2);
                float4 _Property_927253F1_Out_0 = Vector4_B6649628;
                float _Split_28DE5C6_R_1 = _Property_927253F1_Out_0[0];
                float _Split_28DE5C6_G_2 = _Property_927253F1_Out_0[1];
                float _Split_28DE5C6_B_3 = _Property_927253F1_Out_0[2];
                float _Split_28DE5C6_A_4 = _Property_927253F1_Out_0[3];
                float4 _Combine_B3EF3B79_RGBA_4;
                float3 _Combine_B3EF3B79_RGB_5;
                float2 _Combine_B3EF3B79_RG_6;
                Unity_Combine_float(_Split_28DE5C6_R_1, _Split_28DE5C6_G_2, 0, 0, _Combine_B3EF3B79_RGBA_4, _Combine_B3EF3B79_RGB_5, _Combine_B3EF3B79_RG_6);
                float4 _Combine_15195D25_RGBA_4;
                float3 _Combine_15195D25_RGB_5;
                float2 _Combine_15195D25_RG_6;
                Unity_Combine_float(_Split_28DE5C6_B_3, _Split_28DE5C6_A_4, 0, 0, _Combine_15195D25_RGBA_4, _Combine_15195D25_RGB_5, _Combine_15195D25_RG_6);
                float _Remap_A4F7EB75_Out_3;
                Unity_Remap_float(_Power_EA222828_Out_2, _Combine_B3EF3B79_RG_6, _Combine_15195D25_RG_6, _Remap_A4F7EB75_Out_3);
                float _Absolute_7670B6DF_Out_1;
                Unity_Absolute_float(_Remap_A4F7EB75_Out_3, _Absolute_7670B6DF_Out_1);
                float _Smoothstep_F90FF633_Out_3;
                Unity_Smoothstep_float(_Property_31BA0AD9_Out_0, _Property_3C5B259_Out_0, _Absolute_7670B6DF_Out_1, _Smoothstep_F90FF633_Out_3);
                float _Property_C76A5676_Out_0 = Vector1_5ED40136;
                float _Multiply_6EDCD7F2_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_C76A5676_Out_0, _Multiply_6EDCD7F2_Out_2);
                float2 _TilingAndOffset_EFBE272E_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_6EDCD7F2_Out_2.xx), _TilingAndOffset_EFBE272E_Out_3);
                float _Property_2EE1B7C1_Out_0 = Vector1_6BAC9A68;
                float _GradientNoise_4B361D1E_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_EFBE272E_Out_3, _Property_2EE1B7C1_Out_0, _GradientNoise_4B361D1E_Out_2);
                float _Property_E389C913_Out_0 = Vector1_DFA4C9D3;
                float _Multiply_831A5769_Out_2;
                Unity_Multiply_float(_GradientNoise_4B361D1E_Out_2, _Property_E389C913_Out_0, _Multiply_831A5769_Out_2);
                float _Add_C773703A_Out_2;
                Unity_Add_float(_Smoothstep_F90FF633_Out_3, _Multiply_831A5769_Out_2, _Add_C773703A_Out_2);
                float _Add_52522480_Out_2;
                Unity_Add_float(1, _Property_E389C913_Out_0, _Add_52522480_Out_2);
                float _Divide_B9A705F6_Out_2;
                Unity_Divide_float(_Add_C773703A_Out_2, _Add_52522480_Out_2, _Divide_B9A705F6_Out_2);
                float3 _Multiply_9088E467_Out_2;
                Unity_Multiply_float(IN.ObjectSpaceNormal, (_Divide_B9A705F6_Out_2.xxx), _Multiply_9088E467_Out_2);
                float _Property_23A5D0E_Out_0 = Vector1_C7F9BC69;
                float3 _Multiply_A61B043_Out_2;
                Unity_Multiply_float(_Multiply_9088E467_Out_2, (_Property_23A5D0E_Out_0.xxx), _Multiply_A61B043_Out_2);
                float3 _Add_46D18A28_Out_2;
                Unity_Add_float3(IN.ObjectSpacePosition, _Multiply_A61B043_Out_2, _Add_46D18A28_Out_2);
                description.VertexPosition = _Add_46D18A28_Out_2;
                description.VertexNormal = IN.ObjectSpaceNormal;
                description.VertexTangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float3 WorldSpaceNormal;
                float3 WorldSpaceViewDirection;
                float3 WorldSpacePosition;
                float3 AbsoluteWorldSpacePosition;
                float4 ScreenPosition;
                float3 TimeParameters;
            };
            
            struct SurfaceDescription
            {
                float3 Color;
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _Property_C68D1F57_Out_0 = Color_4089CE7A;
                float4 _Property_1751B55B_Out_0 = Color_982234E;
                float _Property_31BA0AD9_Out_0 = Vector1_8E963D5;
                float _Property_3C5B259_Out_0 = Vector1_AECCD54A;
                float4 _Property_5632646D_Out_0 = Vector4_F9BF86A4;
                float _Split_55D0BCE5_R_1 = _Property_5632646D_Out_0[0];
                float _Split_55D0BCE5_G_2 = _Property_5632646D_Out_0[1];
                float _Split_55D0BCE5_B_3 = _Property_5632646D_Out_0[2];
                float _Split_55D0BCE5_A_4 = _Property_5632646D_Out_0[3];
                float3 _RotateAboutAxis_D26A2BC7_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.AbsoluteWorldSpacePosition, (_Property_5632646D_Out_0.xyz), _Split_55D0BCE5_A_4, _RotateAboutAxis_D26A2BC7_Out_3);
                float _Property_81426031_Out_0 = Vector1_62892E34;
                float _Multiply_F54EAFD8_Out_2;
                Unity_Multiply_float(_Property_81426031_Out_0, IN.TimeParameters.x, _Multiply_F54EAFD8_Out_2);
                float2 _TilingAndOffset_166FCC6_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_F54EAFD8_Out_2.xx), _TilingAndOffset_166FCC6_Out_3);
                float _Property_1556047C_Out_0 = Vector1_37F2A07E;
                float _GradientNoise_BA765EAA_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_166FCC6_Out_3, _Property_1556047C_Out_0, _GradientNoise_BA765EAA_Out_2);
                float2 _TilingAndOffset_F368CF6E_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_F54EAFD8_Out_2.xx), _TilingAndOffset_F368CF6E_Out_3);
                float _GradientNoise_578ADE08_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_F368CF6E_Out_3, _Property_1556047C_Out_0, _GradientNoise_578ADE08_Out_2);
                float _Add_EC2BDC4A_Out_2;
                Unity_Add_float(_GradientNoise_BA765EAA_Out_2, _GradientNoise_578ADE08_Out_2, _Add_EC2BDC4A_Out_2);
                float _Divide_24FB0AD0_Out_2;
                Unity_Divide_float(_Add_EC2BDC4A_Out_2, 2, _Divide_24FB0AD0_Out_2);
                float _Saturate_6E0633DE_Out_1;
                Unity_Saturate_float(_Divide_24FB0AD0_Out_2, _Saturate_6E0633DE_Out_1);
                float _Property_C3F02F9F_Out_0 = Vector1_F0BE3006;
                float _Power_EA222828_Out_2;
                Unity_Power_float(_Saturate_6E0633DE_Out_1, _Property_C3F02F9F_Out_0, _Power_EA222828_Out_2);
                float4 _Property_927253F1_Out_0 = Vector4_B6649628;
                float _Split_28DE5C6_R_1 = _Property_927253F1_Out_0[0];
                float _Split_28DE5C6_G_2 = _Property_927253F1_Out_0[1];
                float _Split_28DE5C6_B_3 = _Property_927253F1_Out_0[2];
                float _Split_28DE5C6_A_4 = _Property_927253F1_Out_0[3];
                float4 _Combine_B3EF3B79_RGBA_4;
                float3 _Combine_B3EF3B79_RGB_5;
                float2 _Combine_B3EF3B79_RG_6;
                Unity_Combine_float(_Split_28DE5C6_R_1, _Split_28DE5C6_G_2, 0, 0, _Combine_B3EF3B79_RGBA_4, _Combine_B3EF3B79_RGB_5, _Combine_B3EF3B79_RG_6);
                float4 _Combine_15195D25_RGBA_4;
                float3 _Combine_15195D25_RGB_5;
                float2 _Combine_15195D25_RG_6;
                Unity_Combine_float(_Split_28DE5C6_B_3, _Split_28DE5C6_A_4, 0, 0, _Combine_15195D25_RGBA_4, _Combine_15195D25_RGB_5, _Combine_15195D25_RG_6);
                float _Remap_A4F7EB75_Out_3;
                Unity_Remap_float(_Power_EA222828_Out_2, _Combine_B3EF3B79_RG_6, _Combine_15195D25_RG_6, _Remap_A4F7EB75_Out_3);
                float _Absolute_7670B6DF_Out_1;
                Unity_Absolute_float(_Remap_A4F7EB75_Out_3, _Absolute_7670B6DF_Out_1);
                float _Smoothstep_F90FF633_Out_3;
                Unity_Smoothstep_float(_Property_31BA0AD9_Out_0, _Property_3C5B259_Out_0, _Absolute_7670B6DF_Out_1, _Smoothstep_F90FF633_Out_3);
                float _Property_C76A5676_Out_0 = Vector1_5ED40136;
                float _Multiply_6EDCD7F2_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_C76A5676_Out_0, _Multiply_6EDCD7F2_Out_2);
                float2 _TilingAndOffset_EFBE272E_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_6EDCD7F2_Out_2.xx), _TilingAndOffset_EFBE272E_Out_3);
                float _Property_2EE1B7C1_Out_0 = Vector1_6BAC9A68;
                float _GradientNoise_4B361D1E_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_EFBE272E_Out_3, _Property_2EE1B7C1_Out_0, _GradientNoise_4B361D1E_Out_2);
                float _Property_E389C913_Out_0 = Vector1_DFA4C9D3;
                float _Multiply_831A5769_Out_2;
                Unity_Multiply_float(_GradientNoise_4B361D1E_Out_2, _Property_E389C913_Out_0, _Multiply_831A5769_Out_2);
                float _Add_C773703A_Out_2;
                Unity_Add_float(_Smoothstep_F90FF633_Out_3, _Multiply_831A5769_Out_2, _Add_C773703A_Out_2);
                float _Add_52522480_Out_2;
                Unity_Add_float(1, _Property_E389C913_Out_0, _Add_52522480_Out_2);
                float _Divide_B9A705F6_Out_2;
                Unity_Divide_float(_Add_C773703A_Out_2, _Add_52522480_Out_2, _Divide_B9A705F6_Out_2);
                float4 _Lerp_490B2486_Out_3;
                Unity_Lerp_float4(_Property_C68D1F57_Out_0, _Property_1751B55B_Out_0, (_Divide_B9A705F6_Out_2.xxxx), _Lerp_490B2486_Out_3);
                float _Property_F00B72A1_Out_0 = Vector1_F3A863FF;
                float _FresnelEffect_92852D2_Out_3;
                Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_F00B72A1_Out_0, _FresnelEffect_92852D2_Out_3);
                float _Multiply_1906577B_Out_2;
                Unity_Multiply_float(_Divide_B9A705F6_Out_2, _FresnelEffect_92852D2_Out_3, _Multiply_1906577B_Out_2);
                float _Property_ED8C29D_Out_0 = Vector1_4074C868;
                float _Multiply_4E3A311D_Out_2;
                Unity_Multiply_float(_Multiply_1906577B_Out_2, _Property_ED8C29D_Out_0, _Multiply_4E3A311D_Out_2);
                float4 _Add_B58C5BD9_Out_2;
                Unity_Add_float4(_Lerp_490B2486_Out_3, (_Multiply_4E3A311D_Out_2.xxxx), _Add_B58C5BD9_Out_2);
                float _SceneDepth_7A35D051_Out_1;
                Unity_SceneDepth_Eye_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_7A35D051_Out_1);
                float4 _ScreenPosition_73E54F6_Out_0 = IN.ScreenPosition;
                float _Split_4DE9354B_R_1 = _ScreenPosition_73E54F6_Out_0[0];
                float _Split_4DE9354B_G_2 = _ScreenPosition_73E54F6_Out_0[1];
                float _Split_4DE9354B_B_3 = _ScreenPosition_73E54F6_Out_0[2];
                float _Split_4DE9354B_A_4 = _ScreenPosition_73E54F6_Out_0[3];
                float _Subtract_B3D54811_Out_2;
                Unity_Subtract_float(_Split_4DE9354B_A_4, 1, _Subtract_B3D54811_Out_2);
                float _Subtract_3E15F8E6_Out_2;
                Unity_Subtract_float(_SceneDepth_7A35D051_Out_1, _Subtract_B3D54811_Out_2, _Subtract_3E15F8E6_Out_2);
                float _Property_91FF52CE_Out_0 = Vector1_ED18166C;
                float _Divide_A2748D9B_Out_2;
                Unity_Divide_float(_Subtract_3E15F8E6_Out_2, _Property_91FF52CE_Out_0, _Divide_A2748D9B_Out_2);
                float _Saturate_F15E2859_Out_1;
                Unity_Saturate_float(_Divide_A2748D9B_Out_2, _Saturate_F15E2859_Out_1);
                surface.Color = (_Add_B58C5BD9_Out_2.xyz);
                surface.Alpha = _Saturate_F15E2859_Out_1;
                surface.AlphaClipThreshold = 0.5;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_Position;
                float3 positionWS;
                float3 normalWS;
                float3 viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_Position;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                float3 interp00 : TEXCOORD0;
                float3 interp01 : TEXCOORD1;
                float3 interp02 : TEXCOORD2;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                output.positionCS = input.positionCS;
                output.interp00.xyz = input.positionWS;
                output.interp01.xyz = input.normalWS;
                output.interp02.xyz = input.viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp00.xyz;
                output.normalWS = input.interp01.xyz;
                output.viewDirectionWS = input.interp02.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
            
                output.ObjectSpaceNormal =           input.normalOS;
                output.ObjectSpaceTangent =          input.tangentOS;
                output.ObjectSpacePosition =         input.positionOS;
                output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
                output.TimeParameters =              _TimeParameters.xyz;
            
                return output;
            }
            
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
                output.WorldSpaceNormal =            input.normalWS;
                output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
                output.WorldSpacePosition =          input.positionWS;
                output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
                output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
        
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags 
            { 
                "LightMode" = "ShadowCaster"
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Off
            ZTest LEqual
            ZWrite On
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS 
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS_SHADOWCASTER
            #define REQUIRE_DEPTH_TEXTURE
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 Vector4_F9BF86A4;
            float Vector1_37F2A07E;
            float Vector1_62892E34;
            float Vector1_C7F9BC69;
            float4 Vector4_B6649628;
            float4 Color_982234E;
            float4 Color_4089CE7A;
            float Vector1_8E963D5;
            float Vector1_AECCD54A;
            float Vector1_F0BE3006;
            float Vector1_6BAC9A68;
            float Vector1_5ED40136;
            float Vector1_DFA4C9D3;
            float Vector1_C4072198;
            float Vector1_F3A863FF;
            float Vector1_4074C868;
            float Vector1_ED18166C;
            CBUFFER_END
        
            // Graph Functions
            
            void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
            {
                Rotation = radians(Rotation);
            
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
                
                Axis = normalize(Axis);
            
                float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                          one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                          one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                        };
            
                Out = mul(rot_mat,  In);
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
            
            
            float2 Unity_GradientNoise_Dir_float(float2 p)
            {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            
            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            { 
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
                float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }
            
            void Unity_Power_float(float A, float B, out float Out)
            {
                Out = pow(A, B);
            }
            
            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }
            
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }
            
            void Unity_Absolute_float(float In, out float Out)
            {
                Out = abs(In);
            }
            
            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
            
            void Unity_Add_float3(float3 A, float3 B, out float3 Out)
            {
                Out = A + B;
            }
            
            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
            
            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }
        
            // Graph Vertex
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
                float3 AbsoluteWorldSpacePosition;
                float3 TimeParameters;
            };
            
            struct VertexDescription
            {
                float3 VertexPosition;
                float3 VertexNormal;
                float3 VertexTangent;
            };
            
            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float _Property_31BA0AD9_Out_0 = Vector1_8E963D5;
                float _Property_3C5B259_Out_0 = Vector1_AECCD54A;
                float4 _Property_5632646D_Out_0 = Vector4_F9BF86A4;
                float _Split_55D0BCE5_R_1 = _Property_5632646D_Out_0[0];
                float _Split_55D0BCE5_G_2 = _Property_5632646D_Out_0[1];
                float _Split_55D0BCE5_B_3 = _Property_5632646D_Out_0[2];
                float _Split_55D0BCE5_A_4 = _Property_5632646D_Out_0[3];
                float3 _RotateAboutAxis_D26A2BC7_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.AbsoluteWorldSpacePosition, (_Property_5632646D_Out_0.xyz), _Split_55D0BCE5_A_4, _RotateAboutAxis_D26A2BC7_Out_3);
                float _Property_81426031_Out_0 = Vector1_62892E34;
                float _Multiply_F54EAFD8_Out_2;
                Unity_Multiply_float(_Property_81426031_Out_0, IN.TimeParameters.x, _Multiply_F54EAFD8_Out_2);
                float2 _TilingAndOffset_166FCC6_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_F54EAFD8_Out_2.xx), _TilingAndOffset_166FCC6_Out_3);
                float _Property_1556047C_Out_0 = Vector1_37F2A07E;
                float _GradientNoise_BA765EAA_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_166FCC6_Out_3, _Property_1556047C_Out_0, _GradientNoise_BA765EAA_Out_2);
                float2 _TilingAndOffset_F368CF6E_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_F54EAFD8_Out_2.xx), _TilingAndOffset_F368CF6E_Out_3);
                float _GradientNoise_578ADE08_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_F368CF6E_Out_3, _Property_1556047C_Out_0, _GradientNoise_578ADE08_Out_2);
                float _Add_EC2BDC4A_Out_2;
                Unity_Add_float(_GradientNoise_BA765EAA_Out_2, _GradientNoise_578ADE08_Out_2, _Add_EC2BDC4A_Out_2);
                float _Divide_24FB0AD0_Out_2;
                Unity_Divide_float(_Add_EC2BDC4A_Out_2, 2, _Divide_24FB0AD0_Out_2);
                float _Saturate_6E0633DE_Out_1;
                Unity_Saturate_float(_Divide_24FB0AD0_Out_2, _Saturate_6E0633DE_Out_1);
                float _Property_C3F02F9F_Out_0 = Vector1_F0BE3006;
                float _Power_EA222828_Out_2;
                Unity_Power_float(_Saturate_6E0633DE_Out_1, _Property_C3F02F9F_Out_0, _Power_EA222828_Out_2);
                float4 _Property_927253F1_Out_0 = Vector4_B6649628;
                float _Split_28DE5C6_R_1 = _Property_927253F1_Out_0[0];
                float _Split_28DE5C6_G_2 = _Property_927253F1_Out_0[1];
                float _Split_28DE5C6_B_3 = _Property_927253F1_Out_0[2];
                float _Split_28DE5C6_A_4 = _Property_927253F1_Out_0[3];
                float4 _Combine_B3EF3B79_RGBA_4;
                float3 _Combine_B3EF3B79_RGB_5;
                float2 _Combine_B3EF3B79_RG_6;
                Unity_Combine_float(_Split_28DE5C6_R_1, _Split_28DE5C6_G_2, 0, 0, _Combine_B3EF3B79_RGBA_4, _Combine_B3EF3B79_RGB_5, _Combine_B3EF3B79_RG_6);
                float4 _Combine_15195D25_RGBA_4;
                float3 _Combine_15195D25_RGB_5;
                float2 _Combine_15195D25_RG_6;
                Unity_Combine_float(_Split_28DE5C6_B_3, _Split_28DE5C6_A_4, 0, 0, _Combine_15195D25_RGBA_4, _Combine_15195D25_RGB_5, _Combine_15195D25_RG_6);
                float _Remap_A4F7EB75_Out_3;
                Unity_Remap_float(_Power_EA222828_Out_2, _Combine_B3EF3B79_RG_6, _Combine_15195D25_RG_6, _Remap_A4F7EB75_Out_3);
                float _Absolute_7670B6DF_Out_1;
                Unity_Absolute_float(_Remap_A4F7EB75_Out_3, _Absolute_7670B6DF_Out_1);
                float _Smoothstep_F90FF633_Out_3;
                Unity_Smoothstep_float(_Property_31BA0AD9_Out_0, _Property_3C5B259_Out_0, _Absolute_7670B6DF_Out_1, _Smoothstep_F90FF633_Out_3);
                float _Property_C76A5676_Out_0 = Vector1_5ED40136;
                float _Multiply_6EDCD7F2_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_C76A5676_Out_0, _Multiply_6EDCD7F2_Out_2);
                float2 _TilingAndOffset_EFBE272E_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_6EDCD7F2_Out_2.xx), _TilingAndOffset_EFBE272E_Out_3);
                float _Property_2EE1B7C1_Out_0 = Vector1_6BAC9A68;
                float _GradientNoise_4B361D1E_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_EFBE272E_Out_3, _Property_2EE1B7C1_Out_0, _GradientNoise_4B361D1E_Out_2);
                float _Property_E389C913_Out_0 = Vector1_DFA4C9D3;
                float _Multiply_831A5769_Out_2;
                Unity_Multiply_float(_GradientNoise_4B361D1E_Out_2, _Property_E389C913_Out_0, _Multiply_831A5769_Out_2);
                float _Add_C773703A_Out_2;
                Unity_Add_float(_Smoothstep_F90FF633_Out_3, _Multiply_831A5769_Out_2, _Add_C773703A_Out_2);
                float _Add_52522480_Out_2;
                Unity_Add_float(1, _Property_E389C913_Out_0, _Add_52522480_Out_2);
                float _Divide_B9A705F6_Out_2;
                Unity_Divide_float(_Add_C773703A_Out_2, _Add_52522480_Out_2, _Divide_B9A705F6_Out_2);
                float3 _Multiply_9088E467_Out_2;
                Unity_Multiply_float(IN.ObjectSpaceNormal, (_Divide_B9A705F6_Out_2.xxx), _Multiply_9088E467_Out_2);
                float _Property_23A5D0E_Out_0 = Vector1_C7F9BC69;
                float3 _Multiply_A61B043_Out_2;
                Unity_Multiply_float(_Multiply_9088E467_Out_2, (_Property_23A5D0E_Out_0.xxx), _Multiply_A61B043_Out_2);
                float3 _Add_46D18A28_Out_2;
                Unity_Add_float3(IN.ObjectSpacePosition, _Multiply_A61B043_Out_2, _Add_46D18A28_Out_2);
                description.VertexPosition = _Add_46D18A28_Out_2;
                description.VertexNormal = IN.ObjectSpaceNormal;
                description.VertexTangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float3 WorldSpacePosition;
                float4 ScreenPosition;
            };
            
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float _SceneDepth_7A35D051_Out_1;
                Unity_SceneDepth_Eye_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_7A35D051_Out_1);
                float4 _ScreenPosition_73E54F6_Out_0 = IN.ScreenPosition;
                float _Split_4DE9354B_R_1 = _ScreenPosition_73E54F6_Out_0[0];
                float _Split_4DE9354B_G_2 = _ScreenPosition_73E54F6_Out_0[1];
                float _Split_4DE9354B_B_3 = _ScreenPosition_73E54F6_Out_0[2];
                float _Split_4DE9354B_A_4 = _ScreenPosition_73E54F6_Out_0[3];
                float _Subtract_B3D54811_Out_2;
                Unity_Subtract_float(_Split_4DE9354B_A_4, 1, _Subtract_B3D54811_Out_2);
                float _Subtract_3E15F8E6_Out_2;
                Unity_Subtract_float(_SceneDepth_7A35D051_Out_1, _Subtract_B3D54811_Out_2, _Subtract_3E15F8E6_Out_2);
                float _Property_91FF52CE_Out_0 = Vector1_ED18166C;
                float _Divide_A2748D9B_Out_2;
                Unity_Divide_float(_Subtract_3E15F8E6_Out_2, _Property_91FF52CE_Out_0, _Divide_A2748D9B_Out_2);
                float _Saturate_F15E2859_Out_1;
                Unity_Saturate_float(_Divide_A2748D9B_Out_2, _Saturate_F15E2859_Out_1);
                surface.Alpha = _Saturate_F15E2859_Out_1;
                surface.AlphaClipThreshold = 0.5;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_Position;
                float3 positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_Position;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                float3 interp00 : TEXCOORD0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                output.positionCS = input.positionCS;
                output.interp00.xyz = input.positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp00.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
            
                output.ObjectSpaceNormal =           input.normalOS;
                output.ObjectSpaceTangent =          input.tangentOS;
                output.ObjectSpacePosition =         input.positionOS;
                output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
                output.TimeParameters =              _TimeParameters.xyz;
            
                return output;
            }
            
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
                output.WorldSpacePosition =          input.positionWS;
                output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
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
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags 
            { 
                "LightMode" = "DepthOnly"
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Off
            ZTest LEqual
            ZWrite On
            ColorMask 0
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS 
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS_DEPTHONLY
            #define REQUIRE_DEPTH_TEXTURE
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 Vector4_F9BF86A4;
            float Vector1_37F2A07E;
            float Vector1_62892E34;
            float Vector1_C7F9BC69;
            float4 Vector4_B6649628;
            float4 Color_982234E;
            float4 Color_4089CE7A;
            float Vector1_8E963D5;
            float Vector1_AECCD54A;
            float Vector1_F0BE3006;
            float Vector1_6BAC9A68;
            float Vector1_5ED40136;
            float Vector1_DFA4C9D3;
            float Vector1_C4072198;
            float Vector1_F3A863FF;
            float Vector1_4074C868;
            float Vector1_ED18166C;
            CBUFFER_END
        
            // Graph Functions
            
            void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
            {
                Rotation = radians(Rotation);
            
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
                
                Axis = normalize(Axis);
            
                float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                          one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                          one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                        };
            
                Out = mul(rot_mat,  In);
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
            
            
            float2 Unity_GradientNoise_Dir_float(float2 p)
            {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            
            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            { 
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
                float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }
            
            void Unity_Power_float(float A, float B, out float Out)
            {
                Out = pow(A, B);
            }
            
            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }
            
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }
            
            void Unity_Absolute_float(float In, out float Out)
            {
                Out = abs(In);
            }
            
            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
            
            void Unity_Add_float3(float3 A, float3 B, out float3 Out)
            {
                Out = A + B;
            }
            
            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
            
            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }
        
            // Graph Vertex
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
                float3 AbsoluteWorldSpacePosition;
                float3 TimeParameters;
            };
            
            struct VertexDescription
            {
                float3 VertexPosition;
                float3 VertexNormal;
                float3 VertexTangent;
            };
            
            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float _Property_31BA0AD9_Out_0 = Vector1_8E963D5;
                float _Property_3C5B259_Out_0 = Vector1_AECCD54A;
                float4 _Property_5632646D_Out_0 = Vector4_F9BF86A4;
                float _Split_55D0BCE5_R_1 = _Property_5632646D_Out_0[0];
                float _Split_55D0BCE5_G_2 = _Property_5632646D_Out_0[1];
                float _Split_55D0BCE5_B_3 = _Property_5632646D_Out_0[2];
                float _Split_55D0BCE5_A_4 = _Property_5632646D_Out_0[3];
                float3 _RotateAboutAxis_D26A2BC7_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.AbsoluteWorldSpacePosition, (_Property_5632646D_Out_0.xyz), _Split_55D0BCE5_A_4, _RotateAboutAxis_D26A2BC7_Out_3);
                float _Property_81426031_Out_0 = Vector1_62892E34;
                float _Multiply_F54EAFD8_Out_2;
                Unity_Multiply_float(_Property_81426031_Out_0, IN.TimeParameters.x, _Multiply_F54EAFD8_Out_2);
                float2 _TilingAndOffset_166FCC6_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_F54EAFD8_Out_2.xx), _TilingAndOffset_166FCC6_Out_3);
                float _Property_1556047C_Out_0 = Vector1_37F2A07E;
                float _GradientNoise_BA765EAA_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_166FCC6_Out_3, _Property_1556047C_Out_0, _GradientNoise_BA765EAA_Out_2);
                float2 _TilingAndOffset_F368CF6E_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_F54EAFD8_Out_2.xx), _TilingAndOffset_F368CF6E_Out_3);
                float _GradientNoise_578ADE08_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_F368CF6E_Out_3, _Property_1556047C_Out_0, _GradientNoise_578ADE08_Out_2);
                float _Add_EC2BDC4A_Out_2;
                Unity_Add_float(_GradientNoise_BA765EAA_Out_2, _GradientNoise_578ADE08_Out_2, _Add_EC2BDC4A_Out_2);
                float _Divide_24FB0AD0_Out_2;
                Unity_Divide_float(_Add_EC2BDC4A_Out_2, 2, _Divide_24FB0AD0_Out_2);
                float _Saturate_6E0633DE_Out_1;
                Unity_Saturate_float(_Divide_24FB0AD0_Out_2, _Saturate_6E0633DE_Out_1);
                float _Property_C3F02F9F_Out_0 = Vector1_F0BE3006;
                float _Power_EA222828_Out_2;
                Unity_Power_float(_Saturate_6E0633DE_Out_1, _Property_C3F02F9F_Out_0, _Power_EA222828_Out_2);
                float4 _Property_927253F1_Out_0 = Vector4_B6649628;
                float _Split_28DE5C6_R_1 = _Property_927253F1_Out_0[0];
                float _Split_28DE5C6_G_2 = _Property_927253F1_Out_0[1];
                float _Split_28DE5C6_B_3 = _Property_927253F1_Out_0[2];
                float _Split_28DE5C6_A_4 = _Property_927253F1_Out_0[3];
                float4 _Combine_B3EF3B79_RGBA_4;
                float3 _Combine_B3EF3B79_RGB_5;
                float2 _Combine_B3EF3B79_RG_6;
                Unity_Combine_float(_Split_28DE5C6_R_1, _Split_28DE5C6_G_2, 0, 0, _Combine_B3EF3B79_RGBA_4, _Combine_B3EF3B79_RGB_5, _Combine_B3EF3B79_RG_6);
                float4 _Combine_15195D25_RGBA_4;
                float3 _Combine_15195D25_RGB_5;
                float2 _Combine_15195D25_RG_6;
                Unity_Combine_float(_Split_28DE5C6_B_3, _Split_28DE5C6_A_4, 0, 0, _Combine_15195D25_RGBA_4, _Combine_15195D25_RGB_5, _Combine_15195D25_RG_6);
                float _Remap_A4F7EB75_Out_3;
                Unity_Remap_float(_Power_EA222828_Out_2, _Combine_B3EF3B79_RG_6, _Combine_15195D25_RG_6, _Remap_A4F7EB75_Out_3);
                float _Absolute_7670B6DF_Out_1;
                Unity_Absolute_float(_Remap_A4F7EB75_Out_3, _Absolute_7670B6DF_Out_1);
                float _Smoothstep_F90FF633_Out_3;
                Unity_Smoothstep_float(_Property_31BA0AD9_Out_0, _Property_3C5B259_Out_0, _Absolute_7670B6DF_Out_1, _Smoothstep_F90FF633_Out_3);
                float _Property_C76A5676_Out_0 = Vector1_5ED40136;
                float _Multiply_6EDCD7F2_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_C76A5676_Out_0, _Multiply_6EDCD7F2_Out_2);
                float2 _TilingAndOffset_EFBE272E_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_D26A2BC7_Out_3.xy), float2 (1, 1), (_Multiply_6EDCD7F2_Out_2.xx), _TilingAndOffset_EFBE272E_Out_3);
                float _Property_2EE1B7C1_Out_0 = Vector1_6BAC9A68;
                float _GradientNoise_4B361D1E_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_EFBE272E_Out_3, _Property_2EE1B7C1_Out_0, _GradientNoise_4B361D1E_Out_2);
                float _Property_E389C913_Out_0 = Vector1_DFA4C9D3;
                float _Multiply_831A5769_Out_2;
                Unity_Multiply_float(_GradientNoise_4B361D1E_Out_2, _Property_E389C913_Out_0, _Multiply_831A5769_Out_2);
                float _Add_C773703A_Out_2;
                Unity_Add_float(_Smoothstep_F90FF633_Out_3, _Multiply_831A5769_Out_2, _Add_C773703A_Out_2);
                float _Add_52522480_Out_2;
                Unity_Add_float(1, _Property_E389C913_Out_0, _Add_52522480_Out_2);
                float _Divide_B9A705F6_Out_2;
                Unity_Divide_float(_Add_C773703A_Out_2, _Add_52522480_Out_2, _Divide_B9A705F6_Out_2);
                float3 _Multiply_9088E467_Out_2;
                Unity_Multiply_float(IN.ObjectSpaceNormal, (_Divide_B9A705F6_Out_2.xxx), _Multiply_9088E467_Out_2);
                float _Property_23A5D0E_Out_0 = Vector1_C7F9BC69;
                float3 _Multiply_A61B043_Out_2;
                Unity_Multiply_float(_Multiply_9088E467_Out_2, (_Property_23A5D0E_Out_0.xxx), _Multiply_A61B043_Out_2);
                float3 _Add_46D18A28_Out_2;
                Unity_Add_float3(IN.ObjectSpacePosition, _Multiply_A61B043_Out_2, _Add_46D18A28_Out_2);
                description.VertexPosition = _Add_46D18A28_Out_2;
                description.VertexNormal = IN.ObjectSpaceNormal;
                description.VertexTangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float3 WorldSpacePosition;
                float4 ScreenPosition;
            };
            
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float _SceneDepth_7A35D051_Out_1;
                Unity_SceneDepth_Eye_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_7A35D051_Out_1);
                float4 _ScreenPosition_73E54F6_Out_0 = IN.ScreenPosition;
                float _Split_4DE9354B_R_1 = _ScreenPosition_73E54F6_Out_0[0];
                float _Split_4DE9354B_G_2 = _ScreenPosition_73E54F6_Out_0[1];
                float _Split_4DE9354B_B_3 = _ScreenPosition_73E54F6_Out_0[2];
                float _Split_4DE9354B_A_4 = _ScreenPosition_73E54F6_Out_0[3];
                float _Subtract_B3D54811_Out_2;
                Unity_Subtract_float(_Split_4DE9354B_A_4, 1, _Subtract_B3D54811_Out_2);
                float _Subtract_3E15F8E6_Out_2;
                Unity_Subtract_float(_SceneDepth_7A35D051_Out_1, _Subtract_B3D54811_Out_2, _Subtract_3E15F8E6_Out_2);
                float _Property_91FF52CE_Out_0 = Vector1_ED18166C;
                float _Divide_A2748D9B_Out_2;
                Unity_Divide_float(_Subtract_3E15F8E6_Out_2, _Property_91FF52CE_Out_0, _Divide_A2748D9B_Out_2);
                float _Saturate_F15E2859_Out_1;
                Unity_Saturate_float(_Divide_A2748D9B_Out_2, _Saturate_F15E2859_Out_1);
                surface.Alpha = _Saturate_F15E2859_Out_1;
                surface.AlphaClipThreshold = 0.5;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_Position;
                float3 positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_Position;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                float3 interp00 : TEXCOORD0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                output.positionCS = input.positionCS;
                output.interp00.xyz = input.positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp00.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
            
                output.ObjectSpaceNormal =           input.normalOS;
                output.ObjectSpaceTangent =          input.tangentOS;
                output.ObjectSpacePosition =         input.positionOS;
                output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
                output.TimeParameters =              _TimeParameters.xyz;
            
                return output;
            }
            
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
                output.WorldSpacePosition =          input.positionWS;
                output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
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
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
            ENDHLSL
        }
        
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}
