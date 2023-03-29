sampler uImage0 : register(s0);


float3 uColor;
float uOpacity;
float uSaturation;
float uTime;

texture sampleTexture;
sampler2D samplerTex = sampler_state
{
    texture = <sampleTexture>;
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

texture sampleTexture3;
sampler2D samplerTex3 = sampler_state
{
    texture = <sampleTexture3>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture gradient;
sampler2D gradientTex = sampler_state
{
    texture = <gradient>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 GradientLaserPixel(float4 screenspace : TEXCOORD0) : COLOR0
{
    

    float2 baseUV = screenspace.xy;
    float4 color = tex2D(uImage0, baseUV);

    
    float2 gradUV = float2(baseUV.x - (uTime * 2), baseUV.y);
    float3 gradColor = tex2D(gradientTex, gradUV);
    
    float2 uv1 = float2(baseUV.x - (uTime * 2), baseUV.y);
    float2 uv2 = float2(baseUV.x - (uTime * 1), baseUV.y);
    float2 uv3 = float2(baseUV.x - (uTime * 3), baseUV.y);
    
    float3 tex1 = tex2D(samplerTex, uv1).rgb + gradColor;
    float alpha1 = tex2D(samplerTex, uv1).a;
    
    float3 tex2 = tex2D(samplerTex2, uv1).rgb + gradColor;
    float alpha2 = tex2D(samplerTex, uv2).a;

    float3 tex3 = tex2D(samplerTex3, uv1).rgb + gradColor;
    float alpha3 = tex2D(samplerTex, uv3).a;

    float3 energy = tex1 + tex2 + tex3;
    color.rgb = energy;
    return color;
    
    /*
    
    vec2 gradUV = vec2(UV.x -(TIME * -2.0), UV.y);
	vec3 gradColor = texture(color_gradient, gradUV).rgb;
	
	vec2 uv = vec2(UV.x - (TIME * -2.0), UV.y);
	vec2 uv2 = vec2(UV.x - (TIME * -1.5), UV.y);
	vec2 uv3 = vec2(UV.x - (TIME * -2.5), UV.y);
	
	vec3 tex = texture(noise1_img, uv).rgb + gradColor;// smoke_color.rgb;
	float alpha = texture(noise1_img, uv).a;
	
	vec3 tex2 = texture(secondTex, uv2).rgb + gradColor;//smoke_color.rgb;
	float alpha2 = texture(secondTex, uv2).a;
	
	vec3 tex3 = texture(thirdTex, uv3).rgb + gradColor;//smoke_color.rgb;
	float alpha3 = texture(thirdTex, uv3).a;
	
	vec3 energy = tex + tex2 + tex3;// + smoke_color.rgb;
	float finalAlpha = (tex.r * tex.g * tex.b) + (tex2.r * tex2.g * tex2.b) + (tex3.r * tex3.g * tex3.b);
	
	COLOR.rgb = (energy);// + smoke_color.rgb;
    
    */
    
}

technique Technique1
{
    pass Aura 
    {
        PixelShader = compile ps_2_0 GradientLaserPixel();
    }
}