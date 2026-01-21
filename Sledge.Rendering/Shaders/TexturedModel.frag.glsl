#version 430

struct FragmentIn
{
    vec4 fPosition;
    vec4 fNormal;
    vec3 fTexture;
};

uniform sampler2DArray Texture;

layout(location = 0) in vec4 in_var_NORMAL0;
layout(location = 1) in vec3 in_var_TEXCOORD0;
layout(location = 0) out vec4 out_var_SV_Target0;

vec4 src_main(FragmentIn _input)
{
    vec4 tex = texture(Texture, _input.fTexture);
    if (tex.w < 0.5)
    {
        discard;
    }
    tex.w = 1.0;
    return tex;
}

void main()
{
    vec4 _34 = gl_FragCoord;
    _34.w = 1.0 / gl_FragCoord.w;
    FragmentIn param_var_input = FragmentIn(_34, in_var_NORMAL0, in_var_TEXCOORD0);
    vec4 _38 = src_main(param_var_input);
    out_var_SV_Target0 = _38;
}

