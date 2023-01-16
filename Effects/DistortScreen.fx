sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // Automatically Images/Misc/Perlin via Force Shader testing option
sampler uImage2 : register(s2); // Automatically Images/Misc/noise via Force Shader testing option
sampler uImage3 : register(s3);

texture sampleTexture;
sampler2D samplerTex = sampler_state
{
    texture = <sampleTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
};


texture TileTarget;
sampler tent = sampler_state
{
    Texture = (TileTarget);
};

float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    
    //float4 colour = tex2D(uImage0, uv);
    //float off = uv.x + uv.y * 0.2;
    //float shine = clamp(sin((10.0 * uTime) - off * 15.0), 0.75, 1.0);
    //colour.rgb *= shine;
    //colour.a *= uOpacity;

    //return colour;
    
    //This should be the coords I want 
    //float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    //targetCoords *= (uScreenResolution / uScreenResolution.y);
    //float4 color = tex2D(uImage0, uv);
    //float2 sampleCoords = uv;
    
    //vec2 distortedUV = SCREEN_UV + texture(TEXTURE, UV).a * 0.15;
    //vec4 screenColor = texture(SCREEN_TEXTURE, distortedUV);
    //COLOR = screenColor;
    
    ////float2 distortedUV = tex2D(uImage0, uv).a * 0.05;
    ////float4 screenColor = tex2D(tent, clamp(distortedUV - 0.2f, 0, 0.5));
    ////return screenColor;
    
    
    float4 colour = tex2D(tent, uv);
    colour.r = 100;
    return colour;
    
    
    
    //float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    //float2 centreCoords = (coords - targetCoords) * (uScreenResolution / uScreenResolution.y);
    
}
technique Technique1
{
    pass DistortPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}