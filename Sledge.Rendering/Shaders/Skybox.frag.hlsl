TextureCube Texture;
/*
samplerCUBE Sampler = sampler_state
{
    texture = <Texture>;
};
*/
SamplerState Sampler;

struct PS_INPUT
{
    float4 Position : SV_Position;

    float3 TexCoord : TEXCOORD;
};

float4 main(PS_INPUT input) : SV_Target0
{
    float3 rotatedTexCoord = float3(-input.TexCoord.y, input.TexCoord.z, input.TexCoord.x);
    return Texture.Sample(Sampler, normalize(rotatedTexCoord));
}
