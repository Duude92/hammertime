Texture2D Texture;
SamplerState Sampler;

struct PS_INPUT
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

float4 main(PS_INPUT input) : SV_Target0
{
    return Texture.Sample(Sampler, normalize(input.TexCoord));
}