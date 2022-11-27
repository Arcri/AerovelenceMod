sampler uImage0 : register(s0); // The contents of the screen???

float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float4 uShaderSpecificData;

float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

Texture2D Texture;
sampler TextureSampler
{
    Texture = <Texture>;
};

float2 DisplacementScroll;

Texture2D Displacement;
sampler DisplacementSampler
{
    Texture = <Displacement>;
};

struct vsOutput
{
    float4 position : SV_Position;
    float4 color : COLOR0;
    float2 texCoord : TEXCOORD0;
};

float4 Distort(float4 unused : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{

    float4 v = tex2D(TextureSampler, coords);

    // Look up the displacement amount.
    float2 displacement = tex2D(DisplacementSampler, DisplacementScroll + coords / 3).xy;

    float newCoords = coords + displacement * 0.2 - 0.15;

    // Look up into the main texture.
    return tex2D(TextureSampler, newCoords);// * tex2D(uImage0, coords).rgb;
    
    //float2 disortedUV = tex2D(uImage0, coords).a * 0.1 + ;
    //float4 textureColor = tex2D(uImage0, coords);

    
    //return textureColor;
}

technique Technique1
{
    pass DistortionPulsePass
    {
        PixelShader = compile ps_2_0 Distort();
    }
}