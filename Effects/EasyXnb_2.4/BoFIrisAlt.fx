sampler2D uImage0 : register(s0);

float uTime : register(c0);

float flowSpeed = 0.7;
float vignetteSize = 0.3;
float vignetteBlend = 0.1;
float distortStrength = 0.05;
float xOffset = 0.1;

float squashValue = 0.0;

float colorIntensity = 1.0;

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
    //AddressU = wrap;
    //AddressV = wrap;
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

float4 main(float4 screenspace : TEXCOORD0) : COLOR0
{
    
    float2 baseUV = screenspace.xy;
    
    baseUV.y *= (1.0 - squashValue); // 0.7;
    baseUV.y += squashValue * 0.5; //0.15
    
    float dn = tex2D(distortTex, screenspace.xy + uTime * 0.1).r;
    baseUV += dn * distortStrength;
    baseUV -= distortStrength    / 2.0;
    
    float2 uv;
    uv.x = baseUV.x - (0.5 + xOffset);
    uv.y = baseUV.y - 0.5;
    
    float angle = atan2(uv.y, uv.x);
    float radius = length(uv);
    uv = float2(angle / (2.0 * 3.141592), radius * 2.0);
    uv.y -= uTime;
    //uv is now in polarCoords

    
    float caus = tex2D(causticTex, frac(uv)).r;
    
    float cd = distance(screenspace.xy, float2((0.5 + xOffset), 0.5));
    float vign = 1.0 - smoothstep(vignetteSize, vignetteSize + vignetteBlend, cd);
	
	// Color the caustics
	float grad_uv = caus * vign;
	float3 color = tex2D(gradientTex, float2(grad_uv, grad_uv)).rgb;
	color.rgb = color;
	float alpha = tex2D(uImage0, screenspace.xy).a;
    
    if (alpha == 0)
    {
        color.rgb = float3(0, 0, 0);
    }
    
    float4 toReturn = float4(color * colorIntensity, alpha);
    return toReturn;

    
    /* Old works
    (distort texture did not have wrapping)
    float2 baseUV = screenspace.xy;
    float dn = tex2D(distortTex, screenspace.xy + uTime * 0.1).r;
    baseUV += dn * distortStrength;
    baseUV -= distortStrength    / 2.0;
    
    //float2 uv = baseUV - 0.5;
    float2 uv;
    uv.x = baseUV.x - (0.5 + xOffset);
    uv.y = baseUV.y - 0.5;
    
    float angle = atan2(uv.y, uv.x);
    float radius = length(uv);
    uv = float2(angle / (2.0 * 3.141592), radius * 2.0);
    uv.y -= uTime;
    
    float caus = tex2D(causticTex, frac(uv)).r;
    //uv is now in polarCoords
    
    float cd = distance(screenspace.xy, float2((0.5 + xOffset), 0.5));
    float vign = 1.0 - smoothstep(vignetteSize, vignetteSize + vignetteBlend, cd);
	
	// Color the caustics
	float grad_uv = caus * vign;
	float3 color = tex2D(gradientTex, float2(grad_uv, grad_uv)).rgb;
	color.rgb = color;
	float alpha = tex2D(uImage0, screenspace.xy).a;
    
    if (alpha == 0)
    {
        color.rgb = float3(0, 0, 0);
    }
    
    float4 toReturn = float4(color, alpha);
    return toReturn;
    */
}

technique Technique1
{
    pass Aura
    {
        PixelShader = compile ps_2_0 main();
    }
}
