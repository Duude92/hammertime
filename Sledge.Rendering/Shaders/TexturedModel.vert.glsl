#version 430

struct VertexIn
{
    vec3 Position;
    vec3 Normal;
    vec2 Texture;
    uint Bone;
    uint Flags;
    uint TextureLayer;
};

struct FragmentIn
{
    vec4 fPosition;
    vec4 fNormal;
    vec3 fTexture;
};

layout(binding = 0, std140) uniform Projection
{
    mat4 Selective;
    mat4 Model;
    mat4 View;
    mat4 Projection;
}iProjection;

layout(binding = 1, std140) uniform uTransforms
{
   layout(row_major) mat4 BoneTransforms[128];
};

layout(binding = 2, std140) uniform uLayers
{
    uvec4 TextureRemapTable[16];
};

layout(location = 0) in vec3 in_var_POSITION0;
layout(location = 1) in vec3 in_var_NORMAL0;
layout(location = 2) in vec2 in_var_TEXCOORD0;
layout(location = 3) in uint in_var_COLOR0;
layout(location = 4) in uint in_var_TEXCOORD1;
layout(location = 5) in uint in_var_COLOR1;
layout(location = 0) out vec4 out_var_NORMAL0;
layout(location = 1) out vec3 out_var_TEXCOORD0;
mat4 Identity;
uint Flags_SelectiveTransformed;

FragmentIn src_main(VertexIn _input)
{
    mat4 tModel = transpose(iProjection.Model);
    mat4 tView = transpose(iProjection.View);
    mat4 tProjection = transpose(iProjection.Projection);
    vec4 position = vec4(_input.Position.xyz, 1.0);
    vec4 normal = vec4(_input.Normal.xyz, 1.0);
    mat4 bone = transpose(BoneTransforms[_input.Bone.x]);
    position = bone * position;
    normal = bone * normal ;
    vec4 modelPos = position*tModel;
    modelPos = transpose(iProjection.Selective) * modelPos;
    vec4 cameraPos = modelPos * tView;
    vec4 viewportPos = cameraPos * tProjection;
    FragmentIn _output;
    _output.fPosition = viewportPos;
    _output.fNormal = normal;
    uint layer = TextureRemapTable[_input.TextureLayer / 4u][_input.TextureLayer % 4u];
    _output.fTexture = vec3(_input.Texture, float(layer));
    return _output;
}

void main()
{
    Identity = mat4(vec4(1.0, 0.0, 0.0, 0.0), vec4(0.0, 1.0, 0.0, 0.0), vec4(0.0, 0.0, 1.0, 0.0), vec4(0.0, 0.0, 0.0, 1.0));
    Flags_SelectiveTransformed = 1u;
    VertexIn param_var_input = VertexIn(in_var_POSITION0, in_var_NORMAL0, in_var_TEXCOORD0, in_var_COLOR0, in_var_TEXCOORD1, in_var_COLOR1);
    FragmentIn _69 = src_main(param_var_input);
    gl_Position = _69.fPosition;
    out_var_NORMAL0 = _69.fNormal;
    out_var_TEXCOORD0 = _69.fTexture;
    gl_Position.z = 2.0 * gl_Position.z - gl_Position.w;
}

