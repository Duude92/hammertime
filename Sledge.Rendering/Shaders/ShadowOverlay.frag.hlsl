#define dx 1.0
#define samples 9
#define SHADOWMAP_SIZE 2048
//#define USE_COMPARISON_SAMPLER
struct FragmentIn
{
    float4 fPosition : SV_Position;
    float4 fNormal : NORMAL0;
    float4 fColour : COLOR0;
    float2 fTexture : TEXCOORD0;
    float4 fTint : COLOR1;
    uint1 fFlags : POSITION1;
    float4 sPosition : TEXCOORD1;
};
Texture2D ShadowTexture;
#ifdef USE_COMPARISON_SAMPLER
SamplerComparisonState ShadowSampler;
#else
SamplerState ShadowSampler;
#endif

cbuffer LightData
{
    matrix LightViewProjection;
};
static const float2 PoissonDisk[9] =
{
    float2(-0.94201624, -0.39906216), float2(0.94558609, -0.76890725),
    float2(-0.094184101, -0.92938870), float2(0.34495938, 0.29387760),
    float2(-0.91588581, 0.45771432), float2(-0.81544232, -0.87912464),
    float2(-0.38277543, 0.27676845), float2(0.97484398, 0.75648379),
    float2(0.44323325, -0.97511554)
};
#ifdef USE_COMPARISON_SAMPLER
float ShadowCalculation(float4 shadowCoord, float4 Normal)
{
    shadowCoord = float4(shadowCoord.x, -shadowCoord.y, shadowCoord.z, shadowCoord.w);

    float3 perspectiveShadowCoord = shadowCoord.xyz / shadowCoord.w;
    float2 shadowCoord1 = perspectiveShadowCoord.xy * 0.5 + 0.5;

    float bias = 0.001;
    float currentDepth = perspectiveShadowCoord.z;

    // Hardware PCF: Uses built-in comparison sampler
    return lerp(0.3, 1, ShadowTexture.SampleCmp(ShadowSampler, shadowCoord1, currentDepth - bias));
}

#else
float ShadowCalculation(float4 shadowCoord, float4 Normal)
{

    shadowCoord = float4(shadowCoord.x, -shadowCoord.y, shadowCoord.z, shadowCoord.w);

    float3 perspectiveShadowCoord = shadowCoord.xyz / shadowCoord.w; // Perspective divide
    float2 shadowCoord1 = perspectiveShadowCoord.xy * 0.5 + 0.5; // Transform to 0-1 range
    if (shadowCoord1.x < 0 || shadowCoord1.x > 1 || shadowCoord1.y < 0 || shadowCoord1.y > 1)
        return 1.0; // Fully lit if outside the shadow map
    float bias = 0.001;

    float current = perspectiveShadowCoord.z;
    float shadow = 0.0;

    for (int i = 0; i < samples; i++)
    {
        float closest = ShadowTexture.Sample(ShadowSampler, shadowCoord1.xy + (PoissonDisk[i] * 1 / SHADOWMAP_SIZE)).r;
        if (current > closest - bias)
        {
            shadow += 0.5;
        }

    }
    return 1-(shadow / samples);
}
#endif

float4 main(FragmentIn input) : SV_Target0
{
    return ShadowTexture.Sample(ShadowSampler, input.fTexture.xy).rrra;
    float shadowFactor = ShadowCalculation(input.sPosition, input.fNormal);
    float4 shadowColor = lerp(float4(0, 0, 0, 1), float4(1, 1, 1, 1), shadowFactor);
    return shadowColor;
}
