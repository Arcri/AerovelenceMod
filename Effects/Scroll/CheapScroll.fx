sampler uImage0 : register(s0);

float4 Color1;
float4 Color2;

float Color1Mult;
float Color2Mult;
float totalMult;

float tex1reps;
float tex2reps;

float time1Mult;
float time2Mult;

float satPower;
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

float4 CheapScrollPixel(float4 screenspace : TEXCOORD0) : COLOR0
{
    float2 uv = screenspace.xy;
    float4 input_color = tex2D(uImage0, uv);
	
    //float stretchY = (UV.y * 0.5f) - 0.75f;
    //float stretchY2 = (UV.y * 0.5f) + 0.25f;
    
    float4 col1 = tex2D(samplerTex1, float2(frac(uv.x * tex1reps + (time1Mult * uTime)), uv.y)) * float4(1, 1, 1, 0);
    float4 col2 = tex2D(samplerTex2, float2(frac(uv.x * tex1reps + (time2Mult * uTime)), uv.y)) * float4(1, 1, 1, 0);

    col1 *= Color1 * Color1Mult;
    col2 *= Color2 * Color2Mult;
	
    float4 combined1 = length(col1 + col2) * float4(input_color.rgb * 0.3f, satPower) * input_color.a;
    float4 combined2 = (combined1 * totalMult) + (pow(col1 + col2, float4(2, 2, 2, 2)));
    return combined2;

}

technique Technique1
{
    pass Aura 
    {
        PixelShader = compile ps_2_0 CheapScrollPixel();
    }
}