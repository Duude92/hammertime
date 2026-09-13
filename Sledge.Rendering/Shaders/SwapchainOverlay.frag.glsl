#version 430

struct FInput
{
    vec4 Position;
    vec2 TexCoord;
};

uniform sampler2D resolvedTexture;

layout(location = 0) in vec2 in_var_TEXCOORD0;
layout(location = 0) out vec4 out_var_SV_Target0;

vec4 src_main(FInput Input)
{
    return texture(resolvedTexture, Input.TexCoord);
}

void main()
{
    vec4 _29 = gl_FragCoord;
    _29.w = 1.0 / gl_FragCoord.w;
    FInput param_var_Input = FInput(_29, in_var_TEXCOORD0);
    out_var_SV_Target0 = src_main(param_var_Input);
}

