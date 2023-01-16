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

texture ScreenTarget;
sampler tent = sampler_state
{
    Texture = (ScreenTarget);
};

float4 DistortMiscPixel(float4 screenspace : TEXCOORD0) : COLOR0
{
    
    float2 st = screenspace.xy;
    //float4 color = tex2D(uImage0, st);
    
    float2 distortedUV = tex2D(tent, st).a * 0.05;
    float4 screenColor = tex2D(uImage0, distortedUV);
    return screenColor;

}

technique Technique1
{
    pass DistortPass 
    {
        PixelShader = compile ps_2_0 DistortMiscPixel();
    }
}