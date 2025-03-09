sampler2D TextureSampler : register(s0);
float fadeStart = 0.5f;

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 FadePixelShader(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(TextureSampler, input.TexCoord) * input.Color;
    
    float fadeAmount = saturate((input.TexCoord.y - fadeStart) / (1.0f - fadeStart));
    color.a *= 1.0f - fadeAmount;
    
    return color;
}

technique FadeEffect
{
    pass P0
    {
        PixelShader = compile ps_2_0 FadePixelShader();
    }
}