#version 430

struct FragmentIn
{
    vec4 fPosition;
    vec4 fNormal;
    vec4 fColour;
    vec2 fTexture;
    vec4 fTint;
    uint Flags;
};

layout(location = 0) in
vec4 in_var_NORMAL0;
layout(location = 1) in
vec4 in_var_COLOR0;
layout(location = 2) in
vec2 in_var_TEXCOORD0;
layout(location = 3) in
vec4 in_var_COLOR1;
layout(location = 4) flat in
uint in_var_POSITION1;
layout(location = 0) out
vec4 out_var_SV_Target0;
uint Flags_Wireframed;

vec4 src_main(FragmentIn _input)
{
    if ((_input.Flags & 16u) != 0u)
    {
        return vec4(0.0, 0.0, 1.0, 1.0);
    }
    return _input.fColour;
}

void main()
{
    Flags_Wireframed = 16u;
    vec4 _36 = gl_FragCoord;
    _36.w = 1.0 / gl_FragCoord.w;
    FragmentIn param_var_input = FragmentIn(_36, in_var_NORMAL0, in_var_COLOR0, in_var_TEXCOORD0, in_var_COLOR1, in_var_POSITION1);
    out_var_SV_Target0 = src_main(param_var_input);
}
