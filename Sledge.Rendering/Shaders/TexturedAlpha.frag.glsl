#version 430

struct FragmentIn
{
    vec4 fPosition;
    vec4 fNormal;
    vec4 fColour;
    vec2 fTexture;
    vec4 fTint;
    uint fFlags;
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
layout(location = 4) flat in
uint in_var_POSITION1;
layout(location = 0) out
vec4 out_var_SV_Target0;
vec3 lightDirection;
float lightIntensity;
float ambient;
uint Flags_FlatColour;

vec4 src_main(FragmentIn _input)
{
    vec4 tex = texture(Texture, _input.fTexture) * _input.fTint;
    tex = mix(tex, _input.fTint, vec4(float((_input.fFlags & Flags_FlatColour) / Flags_FlatColour)));
    float incidence = dot(_input.fNormal.xyz, lightDirection);
    float lighting = (lightIntensity * incidence) + ambient;
    vec4 c = tex * lighting;
    c.w = tex.w;
    return c;
}

void main()
{
    lightDirection = vec3(0.267261207103729248046875, 0.534522473812103271484375, 0.80178368091583251953125);
    lightIntensity = 0.5;
    ambient = 0.800000011920928955078125;
    Flags_FlatColour = 2u;
    vec4 _55 = gl_FragCoord;
    _55.w = 1.0 / gl_FragCoord.w;
    FragmentIn param_var_input = FragmentIn(_55, in_var_NORMAL0, in_var_COLOR0, in_var_TEXCOORD0, in_var_COLOR1, in_var_POSITION1);
    out_var_SV_Target0 = src_main(param_var_input);
}




