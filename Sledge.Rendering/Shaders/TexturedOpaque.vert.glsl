#version 430

struct VertexIn
{
    vec3 Position;
    vec3 Normal;
    vec4 Colour;
    vec2 Texture;
    vec4 Tint;
    uint Flags;
};

struct FragmentIn
{
    vec4 fPosition;
    vec4 fNormal;
    vec4 fColour;
    vec2 fTexture;
    vec4 fTint;
    uint fFlags;
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

layout(location = 0) in
vec3 in_var_POSITION0;
layout(location = 1) in
vec3 in_var_NORMAL0;
layout(location = 2) in
vec4 in_var_COLOR0;
layout(location = 3) in
vec2 in_var_TEXCOORD0;
layout(location = 4) in
vec4 in_var_COLOR1;
layout(location = 5) in
uint in_var_POSITION1;
layout(location = 0) out
vec4 out_var_NORMAL0;
layout(location = 1) out
vec4 out_var_COLOR0;
layout(location = 2) out
vec2 out_var_TEXCOORD0;
layout(location = 3) out
vec4 out_var_COLOR1;
layout(location = 4) flat out
uint out_var_POSITION1;
uint Flags_SelectiveTransformed;
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
    mat4 _114 = transpose(spvWorkaroundRowMajor(iProjection.Selective));
    vec4 _123 = vec4(float((_input.Flags & Flags_SelectiveTransformed) / Flags_SelectiveTransformed));
    mat4 _124 = mat4(_123, _123, _123, _123);
    position = mat4(mix(Identity[0], _114[0], _124[0]), mix(Identity[1], _114[1], _124[1]), mix(Identity[2], _114[2], _124[2]), mix(Identity[3], _114[3], _124[3])) * position;
    vec4 modelPos = tModel * position;
    vec4 cameraPos = tView * modelPos;
    vec4 viewportPos = tProjection * cameraPos;
    FragmentIn _output;
    _output.fPosition = viewportPos;
    _output.fNormal = normal;
    _output.fColour = _input.Colour;
    _output.fTexture = _input.Texture;
    _output.fTint = _input.Tint;
    _output.fFlags = _input.Flags;
    return _output;
}

void main()
{
    Flags_SelectiveTransformed = 1u;
    Identity = mat4(vec4(1.0, 0.0, 0.0, 0.0), vec4(0.0, 1.0, 0.0, 0.0), vec4(0.0, 0.0, 1.0, 0.0), vec4(0.0, 0.0, 0.0, 1.0));
    VertexIn param_var_input = VertexIn(in_var_POSITION0, in_var_NORMAL0, in_var_COLOR0, in_var_TEXCOORD0, in_var_COLOR1, in_var_POSITION1);
    FragmentIn _64 = src_main(param_var_input);
    gl_Position = _64.fPosition;
    out_var_NORMAL0 = _64.fNormal;
    out_var_COLOR0 = _64.fColour;
    out_var_TEXCOORD0 = _64.fTexture;
    out_var_COLOR1 = _64.fTint;
    out_var_POSITION1 = _64.fFlags;
    gl_Position.z = 2.0 * gl_Position.z - gl_Position.w;
}

