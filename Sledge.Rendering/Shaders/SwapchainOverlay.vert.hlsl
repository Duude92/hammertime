struct VSOutput
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

struct VertexIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Colour : COLOR0;
    float2 Texture : TEXCOORD0;
    float4 Tint : COLOR1;
    uint1 Flags : POSITION1;
};

VSOutput main(VertexIn input)
{
    VSOutput output;

    output.Position = float4(input.Position, 1.0);
    output.TexCoord = input.Texture;
    return output;
}