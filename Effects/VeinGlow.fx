sampler2D SpriteTexture : register(s0);
    
float2 screenSize;
float glowIntensity;
float glowSpread;
float time;
    
float4 GlowPass(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(SpriteTexture, coords);
        
        // Sample neighboring pixels for the glow effect
    float2 pixelSize = 1.0 / screenSize;
    float4 glow = float4(0, 0, 0, 0);
        
        // Calculate pulsing effect
    float pulse = 0.5 + 0.5 * sin(time * 2.0);
    float spreadAmount = glowSpread * (1.0 + 0.3 * pulse);
        
        // Sample in a circle
    for (int i = 0; i < 8; i++)
    {
        float angle = i * 6.28318 / 8;
        float2 offset = float2(cos(angle), sin(angle)) * spreadAmount * pixelSize;
        glow += tex2D(SpriteTexture, coords + offset);
    }
        
    glow /= 8.0;
    glow *= glowIntensity * (0.7 + 0.3 * pulse);
        
        // Add the glow to the original color
    return color + glow;
}
    
technique Technique1
{
    pass GlowPass
    {
        PixelShader = compile ps_2_0 GlowPass();
    }
}