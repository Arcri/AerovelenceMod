sampler2D uNoise : register(s0);

texture LightTexture;
sampler2D LightSampler = sampler_state
{
    Texture = <LightTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture TerrainTexture;
sampler2D TerrainSampler = sampler_state
{
    Texture = <TerrainTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture FlowTexture;
sampler2D FlowSampler = sampler_state
{
    Texture = <FlowTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

float2 WorldOrigin;
float2 WorldAxisX;
float2 WorldAxisY;
float2 MapOrigin;
float2 MapSize;
float2 WindOffset;
float Time;
float Intensity;
float Daylight;
float Lightning;

float CloudField(float2 world, float depth, float2 bend)
{
    float2 drift = WindOffset * (0.68 + depth * 0.26);
    float2 uv = (world + drift + bend) / float2(850.0, 390.0);
    uv += float2(depth * 0.271, depth * 0.193);
    float2 warp = tex2D(uNoise, uv * 0.67 + float2(Time * 0.003, -Time * 0.005)).rg - 0.5;
    float broad = tex2D(uNoise, uv + warp * 0.52).r;
    float detail = tex2D(uNoise, uv * float2(2.7, 2.1) - warp * 0.34 + float2(-Time * 0.007, Time * 0.006)).g;
    float fine = tex2D(uNoise, uv * float2(6.2, 4.5) + warp * 0.62 + depth * 0.31).b;
    return smoothstep(0.24, 0.77, broad * 0.53 + detail * 0.32 + fine * 0.15);
}

float4 Fog(float4 vertexColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float2 world = WorldOrigin + WorldAxisX * uv.x + WorldAxisY * uv.y;
    float2 mapUV = (world - MapOrigin) / MapSize;
    float4 terrain = tex2D(TerrainSampler, mapUV);
    float4 flow = tex2D(FlowSampler, mapUV);
    float exposure = terrain.r;
    float openSpace = terrain.b;
    float ground = terrain.g;
    float2 bend = (flow.rg * 2.0 - 1.0) * 100.0;
    float clearing = flow.b;
    float rolling = flow.a;
    float2 bankUV = (world + WindOffset * 0.38) / float2(2400.0, 1450.0);
    float2 bankWarp = tex2D(uNoise, bankUV * 0.81 + float2(Time * 0.0011, -Time * 0.0008)).gb - 0.5;
    float bankNoise = tex2D(uNoise, bankUV + bankWarp * 0.29).a;
    float bankCoverage = smoothstep(0.25, 0.75, bankNoise);
    float bankDensity = lerp(0.34, 1.48, bankCoverage);
    float ribbon = tex2D(uNoise, (world + WindOffset * 1.13 + bend * 0.3) / float2(1250.0, 230.0)
        + bankWarp * 0.17).g;
    float groundVariation = lerp(0.65, 1.2, smoothstep(0.25, 0.76, ribbon));

    float2 lightStep = float2(40.0, 40.0) / MapSize;
    float3 localLight = tex2D(LightSampler, mapUV).rgb;
    float3 wideLight = tex2D(LightSampler, mapUV + float2(lightStep.x, 0.0)).rgb;
    wideLight += tex2D(LightSampler, mapUV - float2(lightStep.x, 0.0)).rgb;
    wideLight += tex2D(LightSampler, mapUV + float2(0.0, lightStep.y)).rgb;
    wideLight += tex2D(LightSampler, mapUV - float2(0.0, lightStep.y)).rgb;
    wideLight *= 0.25;
    float3 illumination = max(localLight * 0.62, wideLight * 0.87);
    float luminance = dot(illumination, float3(0.2126, 0.7152, 0.0722));
    float3 ambient = lerp(float3(0.105, 0.14, 0.215), float3(0.43, 0.49, 0.58), Daylight);
    float3 scatter = ambient + illumination * float3(0.5, 0.53, 0.57);
    scatter += Lightning * float3(0.34, 0.40, 0.52);

    float transmission = 1.0;
    float3 accumulated = 0.0;

    [unroll]
    for (int slice = 0; slice < 5; slice++)
    {
        float depth = slice * 0.25;
        float field = CloudField(world, depth, bend * (0.5 + depth * 0.5));
        float raised = CloudField(world - float2(22.0, 36.0), depth, bend * 0.6);
        float body = 0.12 + field * field * 1.8;
        float groundBank = ground * (0.32 + field * 1.9) * groundVariation;
        float displaced = max(0.07, 1.0 - clearing * (0.6 + depth * 0.36));
        float density = (body * 0.045 + groundBank * 0.13) * displaced;
        density += rolling * ground * field * 0.065;
        density *= bankDensity * exposure * lerp(0.11, 1.0, openSpace) * Intensity;
        float extinction = 1.0 - exp(-density);
        float silver = saturate((raised - field) * 2.4 + 0.1);
        float shading = 0.77 + raised * 0.23;
        float3 color = scatter * shading + illumination * silver * 0.24;
        accumulated += transmission * extinction * color;
        transmission *= 1.0 - extinction;
    }

    float veil = exposure * Intensity * (0.018 + ground * 0.016) * lerp(0.65, 1.15, bankCoverage)
        * lerp(0.25, 1.0, openSpace);
    accumulated += transmission * veil * scatter;
    transmission *= 1.0 - veil;
    float halo = saturate(luminance - Daylight * 0.55 - 0.08);
    accumulated += illumination * halo * ground * exposure * openSpace * Intensity * 0.035;
    return float4(accumulated, 1.0 - transmission) * vertexColor;
}

technique CrystalRainFog
{
    pass Mist
    {
        PixelShader = compile ps_3_0 Fog();
    }
}
