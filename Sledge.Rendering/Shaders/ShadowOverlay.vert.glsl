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
    vec4 sPosition;
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

layout(binding = 0, std140) uniform type_LightData
{
    layout(row_major)
mat4 LightView;
}
LightData;

layout(binding = 0, std140) uniform type_LightData2
{
    layout(row_major)
mat4 LightProjection;
}
LightData2;

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
layout(location = 5) out
vec4 out_var_TEXCOORD1;
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
    mat4 _125 = transpose(spvWorkaroundRowMajor(iProjection.Selective));
    vec4 _134 = vec4(float((_input.Flags & Flags_SelectiveTransformed) / Flags_SelectiveTransformed));
    mat4 _135 = mat4(_134, _134, _134, _134);
    position = mat4(mix(Identity[0], _125[0], _135[0]), mix(Identity[1], _125[1], _135[1]), mix(Identity[2], _125[2], _135[2]), mix(Identity[3], _125[3], _135[3])) * position;
    vec4 modelPos = tModel * position;
    vec4 cameraPos = tView * modelPos;
    vec4 viewportPos = tProjection * cameraPos;
    mat4 tLightView = transpose(spvWorkaroundRowMajor(LightData.LightView));
    mat4 tLightProjection = transpose(spvWorkaroundRowMajor(LightData2.LightProjection));
    FragmentIn _output;
    _output.fPosition = viewportPos;
    _output.fNormal = normal;
    _output.fColour = _input.Colour;
    _output.fTexture = _input.Texture;
    _output.fTint = _input.Tint;
    _output.fFlags = _input.Flags;
    _output.sPosition = (tLightProjection * tLightView) * vec4(_input.Position, 1.0);
    return _output;
}

void main()
{
    Flags_SelectiveTransformed = 1u;
    Identity = mat4(vec4(1.0, 0.0, 0.0, 0.0), vec4(0.0, 1.0, 0.0, 0.0), vec4(0.0, 0.0, 1.0, 0.0), vec4(0.0, 0.0, 0.0, 1.0));
    VertexIn param_var_input = VertexIn(in_var_POSITION0, in_var_NORMAL0, in_var_COLOR0, in_var_TEXCOORD0, in_var_COLOR1, in_var_POSITION1);
    FragmentIn _72 = src_main(param_var_input);
    gl_Position = _72.fPosition;
    out_var_NORMAL0 = _72.fNormal;
    out_var_COLOR0 = _72.fColour;
    out_var_TEXCOORD0 = _72.fTexture;
    out_var_COLOR1 = _72.fTint;
    out_var_POSITION1 = _72.fFlags;
    out_var_TEXCOORD1 = _72.sPosition;
    gl_Position.z = 2.0 * gl_Position.z - gl_Position.w;
}

