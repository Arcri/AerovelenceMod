sampler2D uImage0 : register(s0);
float uTime : register(c0);

texture maskTexture;
sampler2D maskTex = sampler_state
{
    texture = <maskTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    //AddressU = wrap;
    //AddressV = wrap;
};

float3 color;

float glowThreshold = 0.4;
float glowPower = 2.5;
float fadeProgress = 1.0;

float endAlpha = 1.0;


float4 main(float4 screenspace : TEXCOORD0) : COLOR0
{
    float2 baseUV = screenspace.xy;

    //Starting color of texture shader is being applied to
    float4 baseCol = tex2D(uImage0, baseUV);
    
    float3 brighten = baseCol.rgb * baseCol.a * color + (baseCol.a > glowThreshold ? ((baseCol.a - glowThreshold) * glowPower) : float3(0, 0, 0));
    
    
    //Subtract using the mask based on fadeProgress 
    float4 maskCol = tex2D(maskTex, baseUV);
    float3 toRet = baseCol.rgb -= (maskCol.rgb * fadeProgress);
    
    float4 toReturn = float4(toRet * brighten * (endAlpha * (1.0 - fadeProgress)), baseCol.a);
    return toReturn;
}

technique Technique1
{
    pass Aura
    {
        PixelShader = compile ps_3_0 main();
    }
}
