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
SamplerState ShadowSampler;

cbuffer LightData
{
    matrix LightViewProjection;
};

float ShadowCalculation(float4 shadowCoord, float4 Normal)
{
    shadowCoord = float4(shadowCoord.x, -shadowCoord.y, shadowCoord.z, shadowCoord.w);
    float3 perspectiveShadowCoord = shadowCoord.xyz / shadowCoord.w; // Perspective divide
    float2 shadowCoord1 = perspectiveShadowCoord.xy * 0.5 + 0.5; // Transform to 0-1 range

    float shadowDepth = ShadowTexture.Sample(ShadowSampler, shadowCoord1.xy).r;
    float bias = 0.001;
    float shadowFactor = (perspectiveShadowCoord.z > shadowDepth - bias) ? 0.5 : 1.0;

    return shadowFactor;
}

float4 main(FragmentIn input) : SV_Target0
{
    float shadowFactor = ShadowCalculation(input.sPosition, input.fNormal);
    float4 shadowColor = lerp(float4(0, 0, 0, 1), float4(1, 1, 1, 1),  shadowFactor);
    return shadowColor;
}
