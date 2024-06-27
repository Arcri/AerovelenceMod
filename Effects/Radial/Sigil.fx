sampler uImage0 : register(s0);

float3 inputColor;
float rotation;
float intensity = 1.0;
float fadeStrength = 1.0;

float glowThreshold = 0.4;

float4 RotateSigilPixel(float4 screenspace : TEXCOORD0) : COLOR0
{
    float2 baseUV = screenspace.xy;
    float4 baseColor = tex2D(uImage0, baseUV);
    
    //Fade out along x
    float alpha = 1.0 - clamp(baseUV.x * fadeStrength, 0.0, 1.0);
    
    //Rotate
    float2x2 rotate = float2x2(cos(rotation), -sin(rotation), sin(rotation), cos(rotation));
    float4 color = tex2D(uImage0, mul((baseUV + float2(-0.5, -0.5)), rotate) + float2(0.5, 0.5));
    
    float3 glow = color.xyz * color.w * inputColor + (color.w > glowThreshold ? ((color.w - glowThreshold) * 2.5) : float3(0, 0, 0));
    float average = ((inputColor.x + inputColor.y + inputColor.z) / 3.0);
    
    return float4(glow, color.a * average * intensity) * (alpha);
    
    //baseColor.rgb = color.rgb + inputColor.rgb;
    //baseColor.a = alpha * color.a * intensity;
    //return baseColor;
}

technique Technique1
{
    pass Aura 
    {
        PixelShader = compile ps_2_0 RotateSigilPixel();
    }
}