#define dx 1.0
#define samples 9
#define SHADOWMAP_SIZE 2048
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
Texture2D ShadowTexture : register(t0, space1);
SamplerState ShadowSampler : register(s1, space1);

cbuffer LightData
{
    matrix LightViewProjection;
};


float4 main(FragmentIn input) : SV_Target0
{
    return ShadowTexture.Sample(ShadowSampler, input.fTexture.xy).rrra;
}
