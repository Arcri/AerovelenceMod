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

float4 RimeLaserPixel(float4 screenspace : TEXCOORD0) : COLOR0
{
    
    float2 st = screenspace.xy;
    float4 color = tex2D(uImage0, st);
    
    float2 uv = float2(st.x + (uTime * 1), st.y);
    float2 uv2 = float2(st.x + (uTime * 1.5), st.y);

    
    float3 tex = tex2D(samplerTex, uv).rgb;//    +uColor.rgb * 2.0;
    float3 tex2 = tex2D(samplerTex2, uv2).rgb;

    float energy = tex + tex2;
    
    color.rgb = energy + uColor;
    color.a = tex2D(uImage0, st).a;
    return color;

}

technique Technique1
{
    pass Aura 
    {
        PixelShader = compile ps_2_0 RimeLaserPixel();
    }
}