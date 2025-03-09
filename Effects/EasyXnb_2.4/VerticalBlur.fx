sampler2D input;

float pixelSize;

float4 VerticalBlur(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(input, texCoord) * 0.4;
    color += tex2D(input, texCoord + float2(0.0, pixelSize)) * 0.2;
    color += tex2D(input, texCoord - float2(0.0, pixelSize)) * 0.2;
    color += tex2D(input, texCoord + float2(0.0, pixelSize * 2.0)) * 0.1;
    color += tex2D(input, texCoord - float2(0.0, pixelSize * 2.0)) * 0.1;
    return color;
}

technique Blur
{
    pass P0
    {
        PixelShader = compile ps_2_0 VerticalBlur();
    }
}