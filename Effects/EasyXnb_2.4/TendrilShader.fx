sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float progress;
float4 ColorOne;
matrix WorldViewProjection;
float4 uShaderSpecificData;
float fadeAmount = 0.0;

struct VertexShaderInput
{
    float3 TextureCoordinates : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float3 TextureCoordinates : TEXCOORD0; //Note the float3 
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};
texture TrailTexture;
sampler tent = sampler_state
{
    Texture = (TrailTexture);
    AddressU = Wrap;
    AddressV = Wrap;
};
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, WorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;

    output.TextureCoordinates = input.TextureCoordinates;

    return output;
};

float4 White(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    
    // Fixes the weird zigzags that can apear when trail width changes | Fix from Dominic/Calamity
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    float x = (coords.x + progress) % 1;
    float2 noisecoords = float2(x, coords.y);
    float brightness = tex2D(tent, noisecoords).r;
    float4 color = ColorOne;
    color *= sqrt(brightness);
    return color;
}

technique BasicColorDrawing
{
    pass DefaultPass
    {
        VertexShader = compile vs_2_0 MainVS();
    }
    pass MainPS
    {
        PixelShader = compile ps_2_0 White();
    }
};