sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
    
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    
    float2 updatedUVpos = float2(coords.x + (sin(uTime) * 0.05), coords.y);
    float2 updatedUVpos2 = float2(coords.x + (cos(uTime) * 0.2), coords.y + (uTime * 0.1));

    float4 color1 = tex2D(uImage1, updatedUVpos);
    float4 color2 = tex2D(uImage1, updatedUVpos2);

    
    float3 combined = color1.rgb + color2.rgb;
    
    color.rgb = combined.rgb * (uColor * 3);
    
    return tex2D(uImage0, coords).a * sampleColor * color;
    
    /*float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    float4 color1 = tex2D(uImage1, coords.xy);

    float readRed = uOpacity * 1.1;

    if (color1.r > readRed)
    {
        color.rgba = 0;
    }
    else if (color1.r > uOpacity)
    {
        color = float4(1, 105.0 / 255, 180.0 / 255, 1);
    }
    return color; */
    
    


}
    
technique Technique1
{
    pass Aura 
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}