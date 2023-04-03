
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float progress;
float4 ColorOne;
matrix WorldViewProjection;
float4 uShaderSpecificData;
int pixelSize;

float pixelation;
float2 resolution;

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
    //float2 grid_uv = round(input.TextureCoordinates * pixelation) / pixelation;
    
    float2 scaledRes = resolution / pixelation;
    float2 modUV = (round(input.TextureCoordinates * scaledRes) / scaledRes);
    
    float x = (modUV.x + progress) % 1;
    float2 noisecoords = float2(x, modUV.y);
    float brightness = tex2D(tent, noisecoords).r;
    float4 color = ColorOne;
    color *= sqrt(brightness);
    color *= sqrt(modUV.x);
    
    return color;
    
    //float2 scaledRes = resolution / pixelation;
    //float2 modUV = (round(input.TextureCoordinates * scaledRes) / scaledRes);
    
    //float2 grid_uv = round(input.TextureCoordinates * pixelation) / pixelation;
    
    //return float4(100, 100, 100, 100);
    
    //float x = (grid_uv.x + progress) % 1;
    //float2 noisecoords = float2(x, grid_uv.y);
    //float brightness = tex2D(tent, noisecoords).r;
    //float4 color = ColorOne;
    //color *= sqrt(brightness);
    //return color * sqrt(grid_uv.x);
    
    
    //Pixelate
    //vec2 grid_uv = round(UV * float(amount)) / float(amount);
    //vec4 text = texture(TEXTURE, grid_uv);
    
    //float x = (input.TextureCoordinates.x + progress) % 1;
    //float2 noisecoords = float2(x, input.TextureCoordinates.y);
    //float brightness = tex2D(tent, noisecoords).r;
    //float4 color = ColorOne;
    //color *= sqrt(brightness);
    //return color * sqrt(input.TextureCoordinates.x);
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