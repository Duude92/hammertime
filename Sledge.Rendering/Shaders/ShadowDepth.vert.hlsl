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


    float4 position = float4(input.Position, 1);
    float4 normal = float4(input.Normal, 1);

    float4 modelPos = mul(position, tModel);
    float4 cameraPos = mul(modelPos, tView);
    float4 viewportPos = mul(cameraPos, tProjection);
    
    PS_INPUT output;
    
    output.Position = viewportPos;
    
    return output;
}
