#version 430

struct PS_INPUT
{
    vec4 pos;
    vec4 col;
    vec2 uv;
};

uniform sampler2D FontTexture;

layout(location = 0) in vec4 in_var_COLOR0;
layout(location = 1) in vec2 in_var_TEXCOORD0;
layout(location = 0) out vec4 out_var_SV_Target;

vec4 src_main(PS_INPUT _input)
{
    vec4 out_col = _input.col * texture(FontTexture, _input.uv);
    return out_col;
}

void main()
{
    vec4 _31 = gl_FragCoord;
    _31.w = 1.0 / gl_FragCoord.w;
    PS_INPUT param_var_input = PS_INPUT(_31, in_var_COLOR0, in_var_TEXCOORD0);
    out_var_SV_Target = src_main(param_var_input);
}

