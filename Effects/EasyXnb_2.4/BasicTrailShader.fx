//Borrowed from SotS WaterTrail

sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float progress;
float4 ColorOne;
matrix WorldViewProjection;
float4 uShaderSpecificData;

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
    float x = (input.TextureCoordinates.x + progress) % 1;
    float2 noisecoords = float2(x, input.TextureCoordinates.y);
    float brightness = tex2D(tent, noisecoords).r;
    float4 color = ColorOne;
    color *= sqrt(brightness);
    return color * sqrt(input.TextureCoordinates.x);
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


/*
float time;
matrix transformMatrix;
float3 uColor;

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

struct VertexShaderInput
{
    float4 Position : POSITION;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    output.Color = input.Color;
    output.TexCoords = input.TexCoords;
    output.Position = mul(input.Position, transformMatrix);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 uv = input.TexCoords;
    float2 updatedUVpos = float2(uv.x, uv.y + (time * 0.01));

    float4 color = input.Color;
    
    float3 newColorRGB = tex2D(samplerTex, updatedUVpos).rgb;
    newColorRGB = newColorRGB * uColor;
    
    return float4(newColorRGB, color.a);
    //float3 color = tex2D(samplerTex, uv + float2(time, 0)).xyz;
    //float3 color2 = tex2D(samplerTex, uv + float2(-time * 1.5, 0)).xyz * 0.5;

    //return float4((color + color2) * input.Color * (1.0 + color.x * 2.0), color.x * input.Color.w);
}

technique Technique1
{
    pass PrimTrail
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};
*/