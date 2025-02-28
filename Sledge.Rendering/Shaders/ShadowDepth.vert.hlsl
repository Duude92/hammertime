cbuffer Projection
{
    matrix Selective;
    matrix Model;
    matrix View;
    matrix Projection;
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

struct PS_INPUT
{
    float4 Position : SV_Position;
};

PS_INPUT main(VertexIn input)
{
    matrix tModel = transpose(Model);
    matrix tView = transpose(View);
    matrix tProjection = transpose(Projection);
    PS_INPUT output;
    output.Position = mul(float4(input.Position, 1), mul(tView, tProjection));
    return output;
}
