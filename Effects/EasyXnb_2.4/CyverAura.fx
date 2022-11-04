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
    float2 st = screenSpace.xy;
    float4 color = tex2D(uImage0, st);

    //float2 updatedUVpos = float2(st.x + (sin(uTime) * 0.05), st.y);
    //float2 updatedUVpos2 = float2(st.x + (cos(uTime) * 0.05), st.y + (cos(uTime) * 0.05));
    
    float2 updatedUVpos = float2(st.x, st.y + uTime * 0.01);
    float2 updatedUVpos2 = float2(st.x, st.y + uTime * 0.01);
    float4 color1 = tex2D(samplerTex, updatedUVpos);
    float4 color2 = tex2D(samplerTex, updatedUVpos2);

    float3 combined = color1.rgb + color2.rgb;

    color.rgb = combined.rgb * (uColor * 1);

    return tex2D(uImage0, st).a  * color;
}
    
technique Technique1
{
    pass Aura 
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}