sampler uImage0 : register(s0);


float3 uColor;
float uOpacity;
float uSaturation;
float uTime;

texture sampleTexture;
sampler2D samplerTex = sampler_state
{
    texture = <sampleTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture sampleTexture2;
sampler2D samplerTex2 = sampler_state
{
    texture = <sampleTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 LaserShaderPixel(float4 screenspace : TEXCOORD0) : COLOR0
{
    
    float2 st = screenspace.xy;
    float4 color = tex2D(uImage0, st);
    
    float2 uv = float2(st.x + (uTime * 0.5), st.y);
    float2 uv2 = float2(st.x + (uTime * 1.0), st.y);

    
    float3 tex = tex2D(samplerTex, uv).rgb;
    float3 tex2 = tex2D(samplerTex2, uv2).rgb;

    float combinedAlpha = (tex.r + tex.g + tex.b) + (tex2.r + tex2.g + tex2.b);
    
    //This being tex 2 and not tex + tex2 is intentional
    color.rgb = tex2 + uColor;
    color.a = tex2D(uImage0, st).a;//  +(combinedAlpha * uSaturation);
    return color;
    
    
    /* old with one texture
    float2 st = screenspace.xy;
    float4 color = tex2D(uImage0, st);
    
    float2 uv = float2(st.x + (uTime * 0.2f), st.y);
    float3 tex = tex2D(samplerTex, uv).rgb;
    
    color.rgb = tex + uColor;
    return color;
    */

}

technique Technique1
{
    pass Aura 
    {
        PixelShader = compile ps_2_0 LaserShaderPixel();
    }
}