Texture2D shadowMap : register(t0);
SamplerState shadowSampler : register(s0);

float4 main(float4 pos : SV_POSITION, float2 uv : TEXCOORD0) : SV_Target
{
    return float4(1.0, 0.0, 0.0, 1.0);
    float shadow = shadowMap.Sample(shadowSampler, uv).r; // Sample shadow map
    float shadowFactor = lerp(1.0, 0.2, shadow); // Darken based on shadow
    return float4(shadowFactor.xxx, 1.0); // Apply grayscale shadow overlay
}