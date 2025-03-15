sampler uImage0 : register(s0);
matrix WorldViewProjection;

float totalMult;

float gradientReps;
float tex1reps;
float tex2reps;
float tex3reps;
float tex4reps;

float tex1Mult;
float tex2Mult;
float tex3Mult;
float tex4Mult;

float grad1Speed;
float grad2Speed;
float grad3Speed;
float grad4Speed;

float satPower;
float3 baseColor;

float uTime;

texture onTex;
sampler2D samplerOnTex = sampler_state
{
    texture = <onTex>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture gradientTex;
sampler2D samplerGradientTex = sampler_state
{
    texture = <gradientTex>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

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
    //Gradient Colors
    float2 UV = input.TextureCoordinates.xy;    
    float4 gradColor1 = tex2D(samplerGradientTex, float2(UV.x * gradientReps + (uTime * grad1Speed), UV.y));
    float4 gradColor2 = tex2D(samplerGradientTex, float2(UV.x * gradientReps + (uTime * grad2Speed), UV.y));
    float4 gradColor3 = tex2D(samplerGradientTex, float2(UV.x * gradientReps + (uTime * grad3Speed), UV.y));
    float4 gradColor4 = tex2D(samplerGradientTex, float2(UV.x * gradientReps + (uTime * grad4Speed), UV.y));
    
    //
    float alpha = tex2D(samplerOnTex, float2(UV.x + (1.0f * uTime), UV.y)).a;
    float4 input_color = float4(baseColor, alpha);
    
    float4 col1 = tex2D(samplerTex1, float2(frac(UV.x * tex1reps + (0.75f * uTime)), UV.y)) * float4(1, 1, 1, 0);
    float4 col2 = tex2D(samplerTex2, float2(frac(UV.x * tex2reps + (1.0f * uTime)), UV.y)) * float4(1, 1, 1, 0);
    float4 col3 = tex2D(samplerTex3, float2(frac(UV.x * tex3reps + (1.25f * uTime)), UV.y)) * float4(1, 1, 1, 0);
    float4 col4 = tex2D(samplerTex4, float2(frac(UV.x * tex4reps + (1.5f * uTime)), UV.y)) * float4(1, 1, 1, 0);
    col1 *= gradColor1 * tex1Mult;
    col2 *= gradColor2 * tex2Mult;
    col3 *= gradColor3 * tex3Mult;
    col4 *= gradColor4 * tex4Mult;
	
    float4 combined1 = length(col1 + col2 + col3 + col4) * float4(input_color.rgb * 0.3f, satPower) * input_color.a;
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