sampler uImage0 : register(s0);
matrix WorldViewProjection;

float4 Color1;
float4 Color2;
float4 Color3;
float4 Color4;
float Color1Mult;
float Color2Mult;
float Color3Mult;
float Color4Mult;
float totalMult;

float tex1reps;
float tex2reps;
float tex3reps;
float tex4reps;

float uTime;

texture sampleTexture1;
sampler2D samplerTex1 = sampler_state
{
    texture = <sampleTexture1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture sampleTexture2;
sampler2D samplerTex2 = sampler_state
{
    texture = <sampleTexture2>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture sampleTexture3;
sampler2D samplerTex3 = sampler_state
{
    texture = <sampleTexture3>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture sampleTexture4;
sampler2D samplerTex4 = sampler_state
{
    texture = <sampleTexture4>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
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

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, WorldViewProjection);
    output.Position = pos;
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
};

float4 ComboLaser(VertexShaderOutput input) : COLOR0
{
    float2 uv = input.TextureCoordinates;
    float4 input_color = input.Color;
	
    float4 col1 = tex2D(samplerTex1, float2(frac(uv.x * tex1reps + (0.75f * uTime)), uv.y)) * float4(1, 1, 1, 0);
    float4 col2 = tex2D(samplerTex2, float2(frac(uv.x * tex1reps + (1.0f * uTime)), uv.y)) * float4(1, 1, 1, 0);
    float4 col3 = tex2D(samplerTex3, float2(frac(uv.x * tex1reps + (1.25f * uTime)), uv.y)) * float4(1, 1, 1, 0);
    float4 col4 = tex2D(samplerTex4, float2(frac(uv.x * tex1reps + (1.5f * uTime)), uv.y)) * float4(1, 1, 1, 0);
    col1 *= Color1 * Color1Mult;
    col2 *= Color2 * Color2Mult;
    col3 *= Color3 * Color3Mult;
    col4 *= Color4 * Color4Mult;
	
    float4 combined1 = length(col1 + col2 + col3 + col4) * float4(input_color.rgb * 0.3f, 0.5f) * input_color.a;
    float4 combined2 = (combined1 * totalMult) + (pow(col1 + col2 + col3 + col4, float4(2, 2, 2, 2)));
    return combined2;

}

technique BasicColorDrawing
{
    pass DefaultPass
    {
        VertexShader = compile vs_2_0 MainVS();
    }
    pass MainPS
    {
        PixelShader = compile ps_2_0 ComboLaser();
    }
};