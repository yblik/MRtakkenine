/*
 * MRUKOcclusion.shader
 *
 * Drop-in replacement for the MRUK EffectMesh outline/line material.
 * Instead of drawing visible lines it writes to the depth buffer only,
 * punching a "hole" in everything rendered on top so real-world geometry
 * occludes virtual objects correctly.
 *
 * HOW TO USE
 * ----------
 *  1. Create a new Material and assign this shader to it.
 *  2. Set that Material on your EffectMesh (or any occlusion mesh) renderer.
 *  3. Make sure the renderer's Sorting Layer / Order in Layer is set to render
 *     BEFORE your virtual objects (lower value = earlier = occluder).
 *  4. Set the MeshRenderer Queue override to "Geometry-1" (2000) or use the
 *     RenderQueue property below so it draws before everything else.
 *
 * RENDER PIPELINE SUPPORT
 * -----------------------
 *  The shader contains two SubShaders:
 *    SubShader 0  -- Universal Render Pipeline (URP)
 *    SubShader 1  -- Built-in Render Pipeline fallback
 *  Unity automatically picks the first one that works in the active pipeline.
 *
 * STENCIL OPTION
 * --------------
 *  Enable _UseStencil and set _StencilRef to write a stencil value as well as
 *  depth. This lets other shaders (e.g. a passthrough tint) test against the
 *  occluder region without an extra depth pass.
 */

Shader "MRUK/Occlusion"
{
    Properties
    {
        [Header(Occlusion Settings)]

        [Enum(UnityEngine.Rendering.ColorWriteMask)]
        _ColorMask ("Color Write Mask", Float) = 0
        // 0 = write nothing to colour (pure depth occluder)
        // 15 = write RGBA (use for debug -- tints the occluded area)

        _DebugTint ("Debug Tint (visible when ColorMask is 15)", Color) = (1, 0, 1, 0.3)

        [Header(Render Order)]
        [IntRange] _RenderQueue ("Render Queue (Geometry = 2000)", Range(1000, 5000)) = 1999

        [Header(Stencil)]
        [Toggle] _UseStencil ("Write Stencil", Float) = 0
        [IntRange] _StencilRef ("Stencil Reference Value", Range(0, 255)) = 1
    }

    // -----------------------------------------------------------------------
    // SUB SHADER 0 -- Universal Render Pipeline
    // -----------------------------------------------------------------------
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"     = "Opaque"
            "Queue"          = "Geometry-1"
        }

        // Depth-only occlusion pass
        Pass
        {
            Name "OcclusionDepth"

            Tags { "LightMode" = "UniversalForward" }

            // Write depth, do not touch colour.
            ZWrite On
            ZTest  LEqual
            ColorMask [_ColorMask]

            // No culling -- occlude from both sides of the mesh.
            Cull Off

            Stencil
            {
                Ref   [_StencilRef]
                Comp  Always
                Pass  Replace
                Fail  Keep
                ZFail Keep
            }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _DebugTint;
                float  _ColorMask;
                float  _UseStencil;
                float  _StencilRef;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Return the debug tint if ColorMask is non-zero,
                // otherwise this output is masked and never written.
                return _DebugTint;
            }
            ENDHLSL
        }
    }

    // -----------------------------------------------------------------------
    // SUB SHADER 1 -- Built-in Render Pipeline fallback
    // -----------------------------------------------------------------------
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue"      = "Geometry-1"
        }

        Pass
        {
            Name "OcclusionDepthBuiltIn"

            ZWrite On
            ZTest  LEqual
            ColorMask [_ColorMask]
            Cull Off

            Stencil
            {
                Ref   [_StencilRef]
                Comp  Always
                Pass  Replace
                Fail  Keep
                ZFail Keep
            }

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            fixed4 _DebugTint;

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _DebugTint;
            }
            ENDCG
        }
    }

    CustomEditor "UnityEditor.ShaderGUI"
}
