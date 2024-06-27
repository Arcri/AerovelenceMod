sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float progress;
float3 ColorOne;
matrix WorldViewProjection;
float4 uShaderSpecificData;

float fadeAmount = 0.0;

float glowThreshold = 0.4;
float glowIntensity = 2.5;


struct VertexShaderInput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float2 TextureCoordinates : TEXCOORD0; 
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
    float2 coords = input.TextureCoordinates.xy;
    
    float x = (coords.x + progress) % 1;
    float2 noisecoords = float2(x, coords.y);
    
    float4 in_color = tex2D(tent, noisecoords);
    
    
    //Brightening part based off of SLR GlowingDust.fx
    
    //Brighten color if above certain threshold
    float3 better_color = in_color.rgb * in_color.a * ColorOne + (in_color.a > glowThreshold ? ((in_color.a - glowThreshold) * glowIntensity) : float3(0, 0, 0));

    //Get average of color
    float average = ((in_color.x + in_color.y + in_color.z) / 3.0);
    
    float input_alpha = input.Color.a;
    
    return float4(better_color, in_color.a * average) * average * input_alpha;

    //return float4(better_color, in_color.a * average) * average * (1.0 - sqrt(input.TextureCoordinates.x));
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
        //VertexShader = compile vs_2_0 MainVS(); //-
    }
};