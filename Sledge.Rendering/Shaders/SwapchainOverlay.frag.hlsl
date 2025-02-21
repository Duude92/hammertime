Texture2D resolvedTexture;
SamplerState textureSampler;

struct FInput
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

float4 main(FInput Input) : SV_Target0
{
    return resolvedTexture.Sample(textureSampler, Input.TexCoord);
}
