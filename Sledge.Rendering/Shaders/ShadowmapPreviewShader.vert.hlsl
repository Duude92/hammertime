struct SizeInfo
{
    float2 Position;
    float2 Size;
};

cbuffer Projection
{
    matrix Projection;
    SizeInfo _SizePos;
};

struct VertexIn
{
    float2 Position : TEXCOORD1;
    float2 Texture : TEXCOORD0;
};

struct PS_INPUT
{
    float2 TexCoord : TEXCOORD0;
    float4 Position : SV_POSITION;
};


PS_INPUT main(VertexIn input)
{
    PS_INPUT output;
    output.TexCoord = input.Texture;
    float2 scaledInput = (input.Position * _SizePos.Size) + _SizePos.Position;
    output.Position = mul(Projection, float4(scaledInput, 0, 1));

    return output;
}
