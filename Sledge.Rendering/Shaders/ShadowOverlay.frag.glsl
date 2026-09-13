#version 430

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

layout(binding = 0, std140) uniform type_LightData
{
    layout(row_major)
mat4 LightViewProjection;
}
LightData;

uniform sampler2D ShadowTexture;

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
layout(location = 5) in
vec4 in_var_TEXCOORD1;
layout(location = 0) out
vec4 out_var_SV_Target0;

vec4 src_main(FragmentIn _input)
{
    return texture(ShadowTexture, _input.fTexture).xxxw;
}

void main()
{
    vec4 _40 = gl_FragCoord;
    _40.w = 1.0 / gl_FragCoord.w;
    FragmentIn param_var_input = FragmentIn(_40, in_var_NORMAL0, in_var_COLOR0, in_var_TEXCOORD0, in_var_COLOR1, in_var_POSITION1, in_var_TEXCOORD1);
    out_var_SV_Target0 = src_main(param_var_input);
}




