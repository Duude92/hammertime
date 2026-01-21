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

struct GeometryIn
{
    vec4 gPosition;
    vec4 gNormal;
    vec4 gColour;
    vec2 gTexture;
    vec4 gTint;
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
uint Flags_SelectiveTransformed;
mat4 Identity;

mat4 spvWorkaroundRowMajor(mat4 wrap)
{
    return wrap;
}

GeometryIn src_main(VertexIn _input)
{
    vec4 position = vec4(_input.Position, 1.0);
    vec4 normal = vec4(_input.Normal, 1.0);
    mat4 _95 = transpose(spvWorkaroundRowMajor(iProjection.Selective));
    vec4 _104 = vec4(float((_input.Flags & Flags_SelectiveTransformed) / Flags_SelectiveTransformed));
    mat4 _105 = mat4(_104, _104, _104, _104);
    position = mat4(mix(Identity[0], _95[0], _105[0]), mix(Identity[1], _95[1], _105[1]), mix(Identity[2], _95[2], _105[2]), mix(Identity[3], _95[3], _105[3])) * position;
    GeometryIn _output;
    _output.gPosition = position;
    _output.gNormal = normal;
    _output.gColour = _input.Colour;
    _output.gTexture = _input.Texture;
    _output.gTint = _input.Tint;
    return _output;
}

void main()
{
    Flags_SelectiveTransformed = 1u;
    Identity = mat4(vec4(1.0, 0.0, 0.0, 0.0), vec4(0.0, 1.0, 0.0, 0.0), vec4(0.0, 0.0, 1.0, 0.0), vec4(0.0, 0.0, 0.0, 1.0));
    VertexIn param_var_input = VertexIn(in_var_POSITION0, in_var_NORMAL0, in_var_COLOR0, in_var_TEXCOORD0, in_var_COLOR1, in_var_POSITION1);
    GeometryIn _62 = src_main(param_var_input);
    gl_Position = _62.gPosition;
    out_var_NORMAL0 = _62.gNormal;
    out_var_COLOR0 = _62.gColour;
    out_var_TEXCOORD0 = _62.gTexture;
    out_var_COLOR1 = _62.gTint;
    gl_Position.z = 2.0 * gl_Position.z - gl_Position.w;
}

