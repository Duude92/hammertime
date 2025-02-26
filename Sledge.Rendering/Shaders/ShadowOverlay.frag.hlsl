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
static const float3 lightDirection = { 0.2672612f, 0.5345225f, 0.8017837f }; // = normalize({ 1, 2, 3 })
static const float lightIntensity = 0.5f;
static const float ambient = 0.8f;




static const uint Flags_FlatColour = 1 << 1;
static const uint Flags_AlphaTested = 1 << 2;

float LinearizeDepth(float depth, float near, float far)
{
    return (depth - near) / (far - near);
}


float ShadowCalculation(float4 shadowCoord)
{
    float3 shx = float3(shadowCoord.x, -shadowCoord.y, shadowCoord.z);
    float3 shadowCoord1 = shx.xyz / shadowCoord.w; // Perspective divide
    shadowCoord1 = shadowCoord1 * 0.5 + 0.5; // Transform to 0-1 range

    float shadowDepth = ShadowTexture.Sample(ShadowSampler, shadowCoord1.xy).r;
    float shadowFactor = (shadowCoord1.z > shadowDepth + 0.3) ? 0.5 : 1.0; // Add bias to avoid artifacts

    return shadowFactor;
}
   

float4 main(FragmentIn input) : SV_Target0
{
    float shadowFactor = ShadowCalculation(input.sPosition);

    
    float4 shadowColor = lerp(float4(0, 0, 0, 1), float4(1, 1, 1, 1),  shadowFactor);

    return shadowColor * input.fTint;
}
