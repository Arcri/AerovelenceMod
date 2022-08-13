sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

  
texture sampleTexture;
sampler2D samplerTex = sampler_state
{
    texture = <sampleTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
};


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

    
float4 PixelShaderFunction(float4 screenSpace : TEXCOORD0) : COLOR0
{
    //Gets the screen coord
    float2 st = screenSpace.xy;
    
    //Gets the color (idk what the abs and fmod does this part if from SLR
    float4 color = tex2D(uImage0, float2(abs(fmod(st.x, 1.0)), st.y));
    
    //If the alpha is is greater than a threshold, add intensity to it
    float3 bright = color.xyz * color.w * uColor + (color.w > uOpacity ? ((color.w - uOpacity) * (2 + (sin(2 * uTime) * uSaturation))) : float3(0, 0, 0)) * 1;
	 
    //Averages out the colors
    float avg = ((uColor.x + uColor.y + uColor.z) / 3.0);

    //Returns the goods
    return float4(bright, color.w * avg) * avg;
}
    
technique Technique1
{
    pass Glow 
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}