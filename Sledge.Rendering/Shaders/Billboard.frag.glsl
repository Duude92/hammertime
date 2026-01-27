#version 430

struct FragmentIn
{
    vec4 fPosition;
    vec4 fNormal;
    vec4 fColour;
    vec2 fTexture;
    vec4 fTint;
};

uniform sampler2D Texture;

layout(location = 0) in
vec4 in_var_NORMAL0;
layout(location = 1) in
vec4 in_var_COLOR0;
layout(location = 2) in
vec2 in_var_TEXCOORD0;
layout(location = 3) in
vec4 in_var_COLOR1;
layout(location = 0) out
vec4 out_var_SV_Target0;

vec4 src_main(FragmentIn _input)
{
    vec4 tex = _input.fColour * texture(Texture, _input.fTexture);
    if (tex.w < 0.0500000007450580596923828125)
    {
        discard;
    }
    return tex * _input.fTint;
}

void main()
{
    vec4 _35 = gl_FragCoord;
    _35.w = 1.0 / gl_FragCoord.w;
    FragmentIn param_var_input = FragmentIn(_35, in_var_NORMAL0, in_var_COLOR0, in_var_TEXCOORD0, in_var_COLOR1);
    vec4 _41 = src_main(param_var_input);
    out_var_SV_Target0 = _41;
}

