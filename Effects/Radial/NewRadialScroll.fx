// Shader based off of: https://nekotoarts.github.io/blog/Polar-Water-Breakdown
const float TAU = 6.283185;

sampler2D uImage0 : register(s0);
float uTime : register(c0);

float flowSpeed = 0.7;
float distortStrength = 0.05;
float colorIntensity = 1.0;

float vignetteSize = 0.2;
float vignetteBlend = 1.0;

texture causticTexture;
sampler2D causticTex = sampler_state
{
    texture = <causticTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    //AddressU = wrap;
    //AddressV = wrap;
};

texture gradientTexture;
sampler2D gradientTex = sampler_state
{
    texture = <gradientTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture distortTexture;
sampler2D distortTex = sampler_state
{
    texture = <distortTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap; //
    AddressV = wrap; //
};

//GLSLs mod function is different than HLSL's fmod function
float2 glslmod(float2 ab, float y)
{
    float xA = ab.x - y * floor(ab.x / y);
    float yB = ab.y - y * floor(ab.y / y);
    return float2(xA, yB);
    
    //return x - y * floor(x / y);
}

float2 polar_coordinates(float2 uv, float2 center, float zoom, float repeat)
{
    float2 dir = uv - center;
    float radius = length(dir) * 2.0;
    float angle = atan2(dir.y, dir.x) / TAU; //atan --> atan2 
    return glslmod(float2(radius * zoom, angle * repeat), 1.0);
    //return mod(float2(radius * zoom, angle * repeat), 1.0);
}

float4 main(float4 screenspace : TEXCOORD0) : COLOR0
{
    //Our normal cartesian coordinates
    float2 baseUV = screenspace.xy;

    //Starting color of texture shader is being applied to
    float4 baseCol = tex2D(uImage0, baseUV);
    
    //Distort our starting UVs using the noise texture
    float distortAmount = tex2D(distortTex, screenspace.xy + uTime * 0.1).r;
    baseUV += distortAmount * distortStrength;
    baseUV -= distortStrength / 2.0;
    
    
    //Convert the distorted UVs into polar coordinates (r, theta)
    float2 polarUV = polar_coordinates(baseUV, float2(0.5, 0.5), 1.0, 1.0);
    polarUV.x -= uTime * flowSpeed;
    
    //Get the color of the caustic texture at the polar coords (All input textures should be grayscale so rgb are all equal)
    float caus = tex2D(causticTex, polarUV).r;

    //Vignette
    float cd = distance(screenspace.xy, float2(0.5, 0.5));
    float vign = 1.0 - smoothstep(vignetteSize, vignetteSize + vignetteBlend, cd);
    
    //Color the caustic texture using the gradient
    float gradUV = caus * vign;
    float4 causColor = tex2D(gradientTex, float2(0.005 + (gradUV * 0.99), 0.0));
    //Above we do '0.005 * gradUV * 0.99' instead of just 'gradUV' because otherwise the shader has this weird issue that I think has to do with the texture wrapping
    //Either when it should be drawing the rightmost color it would draw the leftmost or vice-versa idk it was really strange but this fixed it.
    
    float4 toReturn = float4(causColor.rgb * colorIntensity, baseCol.a);
    return toReturn;
}

technique Technique1
{
    pass Aura
    {
        PixelShader = compile ps_2_0 main();
    }
}
