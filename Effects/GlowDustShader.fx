﻿sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
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

float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;
float2 uTargetPosition;

//uOpacity = threshold
//uSaturaiton = strength


float4 ArmorBasic(float2 coords : TEXCOORD0) : COLOR0
{
    //Gets the color (idk what the abs and fmod does this part if from SLR    
    float4 color = tex2D(uImage0, float2(abs(fmod(coords.x, 1.0)), coords.y));
    
    //If the alpha is is greater than a threshold, add intensity to it
    float3 bright = color.xyz * color.w * uColor + (color.w > uOpacity ? ((color.w - uOpacity) * (2.5 + (sin(2 * uTime) * uSaturation))) : float3(0, 0, 0)) * 1;
	 
    //Averages out the colors
    float avg = ((uColor.x + uColor.y + uColor.z) / 3.0);

    //Returns the goods
    return float4(bright, color.w * avg) * avg;
    
}


    
    
technique Technique1
{
    pass ArmorBasic
    {
        PixelShader = compile ps_2_0 ArmorBasic();
    }
}