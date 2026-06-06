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

struct VSOutput
{
    vec4 Position;
    vec2 TexCoord;
};

layout(location = 0) in vec3 in_var_POSITION0;
layout(location = 1) in vec3 in_var_NORMAL0;
layout(location = 2) in vec4 in_var_COLOR0;
layout(location = 3) in vec2 in_var_TEXCOORD0;
layout(location = 4) in vec4 in_var_COLOR1;
layout(location = 5) in uint in_var_POSITION1;
layout(location = 0) out vec2 out_var_TEXCOORD0;

VSOutput src_main(VertexIn _input)
{
    VSOutput _output;
    _input.Position.y = -_input.Position.y;
    _output.Position = vec4(_input.Position, 1.0);
    _output.TexCoord = _input.Texture;
    return _output;
}

void main()
{
    VertexIn param_var_input = VertexIn(in_var_POSITION0, in_var_NORMAL0, in_var_COLOR0, in_var_TEXCOORD0, in_var_COLOR1, in_var_POSITION1);
    VSOutput _40 = src_main(param_var_input);
    gl_Position = _40.Position;
    out_var_TEXCOORD0 = _40.TexCoord;
    gl_Position.z = 2.0 * gl_Position.z - gl_Position.w;
}

