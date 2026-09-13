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
    vec2 fTexture;
    uint fBone;
};

layout(binding = 0, std140) uniform Projection
{
    layout(row_major)
mat4 Selective;
    layout(row_major)
mat4 Model;
    layout(row_major)
mat4 View;
    layout(row_major)
mat4 Projection;
}
iProjection;

layout(binding = 1, std140) uniform uTransforms
{
    layout(row_major) mat4 BoneTransforms[128];
};

layout(location = 0) in
vec3 in_var_POSITION0;
layout(location = 1) in
vec3 in_var_NORMAL0;
layout(location = 2) in
vec2 in_var_TEXCOORD0;
layout(location = 3) in
uint in_var_COLOR0;
layout(location = 4) in
uint in_var_TEXCOORD1;
layout(location = 5) in
uint in_var_COLOR1;
layout(location = 0) out
vec4 out_var_NORMAL0;
layout(location = 1) out
vec2 out_var_TEXCOORD0;
layout(location = 2) flat out
uint out_var_POSITION1;
mat4 Identity;

mat4 spvWorkaroundRowMajor(mat4 wrap)
{
    return wrap;
}

FragmentIn src_main(VertexIn _input)
{
    mat4 tModel = transpose(spvWorkaroundRowMajor(iProjection.Model));
    mat4 tView = transpose(spvWorkaroundRowMajor(iProjection.View));
    mat4 tProjection = transpose(spvWorkaroundRowMajor(iProjection.Projection));
    vec4 position = vec4(_input.Position, 1.0);
    vec4 normal = vec4(_input.Normal, 1.0);
    mat4 bone = transpose(spvWorkaroundRowMajor(BoneTransforms[_input.Bone]));
    position = bone * position;
    normal = bone * normal;
    vec4 modelPos = tModel * position;
    vec4 cameraPos = tView * modelPos;
    vec4 viewportPos = tProjection * cameraPos;
    FragmentIn _output;
    _output.fPosition = viewportPos;
    _output.fNormal = normal;
    _output.fTexture = _input.Texture;
    _output.fBone = _input.Bone;
    _output.fPosition.z = 0.0;
    return _output;
}

void main()
{
    Identity = mat4(vec4(1.0, 0.0, 0.0, 0.0), vec4(0.0, 1.0, 0.0, 0.0), vec4(0.0, 0.0, 1.0, 0.0), vec4(0.0, 0.0, 0.0, 1.0));
    VertexIn param_var_input = VertexIn(in_var_POSITION0, in_var_NORMAL0, in_var_TEXCOORD0, in_var_COLOR0, in_var_TEXCOORD1, in_var_COLOR1);
    FragmentIn _61 = src_main(param_var_input);
    gl_Position = _61.fPosition;
    out_var_NORMAL0 = _61.fNormal;
    out_var_TEXCOORD0 = _61.fTexture;
    out_var_POSITION1 = _61.fBone;
    gl_Position.z = 2.0 * gl_Position.z - gl_Position.w;
}

