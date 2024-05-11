
struct WorldAndInverse
{
    float4x4 World;
    float4x4 InverseWorld;
};


struct VertexIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Texture : TEXCOORD0;
    float4x4 _ViewProjection;
    WorldAndInverse _WorldAndInverse;
};

struct PS_INPUT
{
    float4 Position : SV_Position;
    float3 TexCoord : TEXCOORD0;
};

PS_INPUT main(VertexIn input)
{
    PS_INPUT output;
    output.Position = mul(input._ViewProjection, mul(input._WorldAndInverse.World, float4(input.Position, 1)));
    output.Position.y = input.Position.y * .0001f;
    
    return output;
}
