sampler uImage0 : register(s0);

float rotation;
float rainbowRotation = 0.0;

float intensity = 5.0;
float fadeStrength = 1.0;


const float TWO_PI = 6.28318530718;


//Converts hsv to rgb
float3 hsv2rgb(float3 _c)
{
    float4 _K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 _p = abs(frac(_c.xxx + _K.xyz) * 6.0 - _K.www);
    return _c.z * lerp(_K.xxx, clamp(_p - _K.xxx, 0.0, 1.0), _c.y);
}

float4 RotateSigilPixel(float4 screenspace : TEXCOORD0) : COLOR0
{
    float2 baseUV = screenspace.xy;
    float4 baseColor = tex2D(uImage0, baseUV);
 
    //float2 pos = float2(0.5, 0.5) - baseUV;
    //baseColor.rgb += intensity * hsv2rgb(float3(((atan2(pos.y, pos.x) + rainbowRotation) / TWO_PI) + 0.5, length(pos) * 2.0, 1.0));
    //baseColor.rgb = intensity * hsv2rgb(float3(((atan2(pos.y, pos.x) + rainbowRotation) / TWO_PI) + 0.5, length(pos) * 2.0, 1.0));
    //return baseColor;
    
    //Fade out along x
    float alpha = 1.0 - clamp(baseUV.x * fadeStrength, 0.0, 1.0);
    
    //Rotate
    float2x2 rotate = float2x2(cos(rotation), -sin(rotation), sin(rotation), cos(rotation));
    float4 color = tex2D(uImage0, mul((baseUV + float2(-0.5, -0.5)), rotate) + float2(0.5, 0.5));
    
    color.a = alpha * color.a;
    
    float4 toReturn = color;
    
    float2 pos = float2(0.5, 0.5) - baseUV;
    toReturn.rgb = intensity * hsv2rgb(float3(((atan2(pos.y, pos.x) + rainbowRotation) / TWO_PI) + 0.5, length(pos) * 2.0, 1.0));
    
    return toReturn;
}

technique Technique1
{
    pass Aura 
    {
        PixelShader = compile ps_2_0 RotateSigilPixel();
    }
}