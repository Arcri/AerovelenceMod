sampler uImage0 : register(s0);

//!!!!!! This Shader does not work as intended and was
//!!!!!! one of the situations where even though it was fucked it look unique


float3 uColor;
float uTime;

texture caustics;
sampler2D causticTex = sampler_state
{
    texture = <caustics>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture distort;
sampler2D distortNoise = sampler_state
{
    texture = <distort>;
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

uniform float flow_speed = 0.3;
uniform float vignette_size = -0.3;
uniform float vignette_blend = 0.3; //0.1
uniform float distort_strength = 0; //0.1
uniform float disc_speed = 0.5;

//Value of 2pi
const float TAU = 6.283185307;

float2 glslMod(float2 x, float y)
{
    float2 returnLater = (x - y * floor(x / y));
    return returnLater;
}

float2 polar_coordinates(float2 uv, float2 center, float zoom, float repeat)
{
    float2 dir = uv - center;
    float radius = length(dir) * 2.0;
    float angle = atan(dir) / TAU;
    return glslMod(float2(radius * zoom, angle * repeat), 1.0);
    //return modf(float2(radius * zoom, angle * repeat), 1.0);
}



float4 main(float4 screenspace : TEXCOORD0) : COLOR0
{
 
    float2 st = screenspace.xy;
    float dn = tex2D(distortNoise, st + (uTime * 0.1)).r;
    st += dn * distort_strength;
    st -= distort_strength / 2.0;
    
    float2 polar_uv = polar_coordinates(st, float2(0.5, 0.5), 1.0, 1.0);
    polar_uv.x -= uTime * flow_speed;
    float caus = tex2D(causticTex, polar_uv).r;
    
    //fade out Caustics
    ///float cd = distance(screenspace.xy, float2(0.5, 0.5));
    ///float vign = 1.0 - smoothstep(vignette_size, vignette_size + vignette_blend, cd);
    
    //color the caustics
    ///float grad_uv = caus * vign;
    float3 color = tex2D(gradientTex, float2(caus, caus)).rgb;
    
    return float4(color, tex2D(uImage0, st).a);
    
    /*
    float2 st = screenspace.xy;
    float dn = tex2D(distortNoise, st + (uTime * 0.1)).r;
    st += dn * distort_strength;
    st -= distort_strength / 2.0;
    
    float2 polar_uv = polar_coordinates(st, float2(0.5, 0.5), 1.0, 1.0);
    polar_uv.x -= uTime * flow_speed;
    float caus = tex2D(causticTex, polar_uv).r;
    
    //fade out Caustics
    float cd = distance(screenspace.xy, float2(0.5, 0.5));
    float vign = 1.0 - smoothstep(vignette_size, vignette_size + vignette_blend, cd);
    
    //color the caustics
    float grad_uv = caus * vign;
    float3 color = tex2D(gradientTex, float2(grad_uv)).rgb;
    
    return float4(color, tex2D(uImage0, screenspace.xy).a);
    */
    
}

technique Technique1
{
    pass Aura 
    {
        PixelShader = compile ps_2_0 main();
    }
}