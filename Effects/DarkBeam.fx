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
    texture = <sampleTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};


float Strand(float2 uv, float hoffset, float hscale, float vscale, float t)
{
    float g = .06;
    float d = 1. - abs(uv.y - (sin(uv.x * hscale * 10. + t + hoffset) / 4. * vscale
                                + .5));
    return clamp(d, 0., 1.)
            + clamp(1. + d / g, 0., 1.) * .4;
}

/*
vec4 Muzzle(float2 uv)
{
    vec2 u = (R * vec2(1, .5) - uv) / R.y;
    float T = floor(iGlobalTime * 20.),
      theta = atan(u.y, u.x),
	    len = (10. + sin(theta * 20. - T * 35.)) / 11.;
    u.y *= 4.;
    float d = max(-0.6, 1. - length(u) / len);
    return d * (1. + .5 * vec4(sin(theta * 10. + T * 10.77),
                          -cos(theta * 8. - T * 8.77),
                          -sin(theta * 6. - T * 134.77),
                           0));
}
*/



//shader not used at all 
float4 LaserShaderPixel(float4 screenspace : TEXCOORD0) : COLOR0
{
    
    float2 st = screenspace.xy;
    float4 color = tex2D(uImage0, st);
    
    
    float t = 4. * uTime,
          s = 1. + sin(uTime) * 5.;
    
    float3 c = float3(0, 0, 0);
    c += Strand(st, 0.234 + s, 1.0, 0.16, 10.0 * t) * float4(1, 0, 0, 0);
    c += Strand(st, 0.645 + s, 1.5, 0.20, 8.3 * t) * float4(0, 1, 0, 0);
    c += Strand(st, 1.735 + s, 1.3, 0.19, 8.0 * t) * float4(0, 0, 1, 0);
    c += Strand(st, 0.9245 + s, 1.6, 0.14, 12.0 * t) * float4(1, 1, 0, 0);
    c += Strand(st, 0.4234 + s, 1.9, 0.23, 14.0 * t) * float4(0, 1, 1, 0);
    c += Strand(st, 0.14525 + s, 1.2, 0.18, 9.0 * t) * float4(1, 0, 1, 0);
    
    return float4(c, color.a);

}

technique Technique1
{
    pass Aura 
    {
        PixelShader = compile ps_2_0 LaserShaderPixel();
    }
}