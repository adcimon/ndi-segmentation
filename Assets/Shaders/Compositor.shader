Shader "Compositor"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _SourceTexture;
    sampler2D _MaskTexture;
    float4 _BackgroundColor;
    sampler2D _BackgroundTexture;

    void Vertex( float4 position : POSITION, float2 uv : TEXCOORD0, out float4 outPosition : SV_Position, out float2 outUV : TEXCOORD0 )
    {
        outPosition = UnityObjectToClipPos(position);
        outUV = uv;
    }

    float4 CompositeForeground( float2 uv, float3 bg )
    {
        float3 fg = tex2D(_SourceTexture, uv).rgb;
        float mask = tex2D(_MaskTexture, uv).r;

        // Overlay blend mode.
        float3 bl = fg < 0.5 ? 2 * bg * fg : 1 - 2 * (1 - bg) * (1 - fg);

        // Interpolation.
        float3 rgb = bg;
        rgb = lerp(rgb, bl, saturate((mask - 0.1) / 0.4));
        rgb = lerp(rgb, fg, saturate((mask - 0.5) / 0.4));
        return float4(rgb , 1);
    }

    float4 FragmentSource( float4 position : SV_Position, float2 uv : TEXCOORD0 ) : SV_Target
    {
        return tex2D(_SourceTexture, uv);
    }

    float4 FragmentMask( float4 position : SV_Position, float2 uv : TEXCOORD0 ) : SV_Target
    {
        float4 color = tex2D(_SourceTexture, uv);
        float mask = tex2D(_MaskTexture, uv).r;
        return lerp(color, float4(1, 0, 0, 1), mask * 0.8);
    }

    float4 FragmentColor(float4 position : SV_Position, float2 uv : TEXCOORD0) : SV_Target
    {
        return CompositeForeground(uv, _BackgroundColor.rgb);
    }

    float4 FragmentTexture( float4 position : SV_Position, float2 uv : TEXCOORD0 ) : SV_Target
    {
        return CompositeForeground(uv, tex2D(_BackgroundTexture, uv).rgb);
    }

    ENDCG

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentSource
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentMask
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentColor
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentTexture
            ENDCG
        }
    }
}