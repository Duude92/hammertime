struct VertexIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Texture : TEXCOORD0;
    uint1 Bone : COLOR0;
    uint1 Flags : TEXCOORD1;
    nointerpolation uint TextureLayer : COLOR1;
};

struct FragmentIn
{
    float4 fPosition : SV_Position;
    float4 fNormal : NORMAL0;
    float3 fTexture : TEXCOORD0;
};

cbuffer Projection
{
    matrix Selective;
    matrix Model;
    matrix View;
    matrix Projection;
};

cbuffer BoneTransforms
{
    matrix uTransforms[128];
};
cbuffer TextureRemapTable
{
    uint4 uLayers[16];
};

static const float4x4 Identity = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
static const uint Flags_SelectiveTransformed = 1 << 0;

FragmentIn main(VertexIn input)
{
    matrix tModel = transpose(Model);
    matrix tView = transpose(View);
    matrix tProjection = transpose(Projection);

    FragmentIn output;

    float4 position = float4(input.Position, 1);
    float4 normal = float4(input.Normal, 1);
    
    matrix bone = transpose(uTransforms[input.Bone.x]);
    position = mul(position, bone);
    normal = mul(normal, bone);

    float4 modelPos = mul(position, tModel);
    modelPos = mul(modelPos, transpose(Selective));
    float4 cameraPos = mul(modelPos, tView);
    float4 viewportPos = mul(cameraPos, tProjection);

    output.fPosition = viewportPos;
    output.fNormal = normal;

    uint layer = uLayers[input.TextureLayer / 4][input.TextureLayer % 4];
    output.fTexture = float3(input.Texture, layer);

    return output;
}
