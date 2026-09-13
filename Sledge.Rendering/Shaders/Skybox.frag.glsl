#version 430

struct PS_INPUT
{
    vec4 Position;
    vec3 TexCoord;
};

uniform samplerCube Texture;

layout(location = 0) in
vec3 in_var_TEXCOORD;
layout(location = 0) out
vec4 out_var_SV_Target0;

vec4 src_main(PS_INPUT _input)
{
    vec3 rotatedTexCoord = vec3(-_input.TexCoord.y, _input.TexCoord.z, _input.TexCoord.x);
    return texture(Texture, normalize(rotatedTexCoord));
}

void main()
{
    vec4 _32 = gl_FragCoord;
    _32.w = 1.0 / gl_FragCoord.w;
    PS_INPUT param_var_input = PS_INPUT(_32, in_var_TEXCOORD);
    out_var_SV_Target0 = src_main(param_var_input);
}

