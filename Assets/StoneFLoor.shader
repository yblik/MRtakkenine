Shader "Custom/StoneGlowPulse"
{
    Properties
    {
        _MainTex ("Stone Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _GlowColor ("Glow Color", Color) = (0,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0,5)) = 1.5
        _PulseSpeed ("Pulse Speed", Range(0,10)) = 2.0
        _PulseStrength ("Pulse Strength", Range(0,2)) = 1.0
        _GlowMask ("Glow Mask", 2D) = "black" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _GlowMask;

        fixed4 _GlowColor;
        float _GlowIntensity;
        float _PulseSpeed;
        float _PulseStrength;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_GlowMask;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Base stone texture
            fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = col.rgb;

            // Normal map
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));

            // Glow mask (where glow appears)
            float glowMask = tex2D(_GlowMask, IN.uv_GlowMask).r;

            // Time-based pulse
            float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;

            // Apply pulse strength
            pulse = lerp(1.0 - _PulseStrength, 1.0 + _PulseStrength, pulse);

            // Final emission (glow)
            o.Emission = _GlowColor.rgb * glowMask * _GlowIntensity * pulse;

            // Smoothness for stone
            o.Smoothness = 0.2;
            o.Metallic = 0.0;
        }
        ENDCG
    }

    FallBack "Standard"
}