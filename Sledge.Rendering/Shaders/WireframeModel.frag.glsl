#version 430

struct FragmentIn
{
    vec4 fPosition;
    vec4 fNormal;
    vec2 fTexture;
    uint fBone;
};

layout(location = 0) in
vec4 in_var_NORMAL0;
layout(location = 1) in
vec2 in_var_TEXCOORD0;
layout(location = 2) flat in
uint in_var_POSITION1;
layout(location = 0) out
vec4 out_var_SV_Target0;

vec4 src_main(FragmentIn _input)
{
    return vec4(1.0);
}

void main()
{
    vec4 _26 = gl_FragCoord;
    _26.w = 1.0 / gl_FragCoord.w;
    FragmentIn param_var_input;
    out_var_SV_Target0 = src_main(param_var_input);
}

