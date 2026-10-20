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
uint Flags_AlphaTested;

vec4 src_main(FragmentIn _input)
{
    vec4 tex = texture(Texture, _input.fTexture);
    tex = mix(tex, _input.fColour, vec4(float((_input.fFlags & Flags_FlatColour) / Flags_FlatColour)));
    tex.w = mix(1.0, tex.w, float((_input.fFlags & Flags_AlphaTested) / Flags_AlphaTested));
    if (tex.w < 0.5)
    {
        discard;
    }
    tex.w = 1.0;
    float incidence = dot(_input.fNormal.xyz, lightDirection);
    float lighting = (lightIntensity * incidence) + ambient;
    vec4 c = tex * lighting;
    c.w = tex.w;
    return c * _input.fTint;
}

void main()
{
    lightDirection = vec3(0.267261207103729248046875, 0.534522473812103271484375, 0.80178368091583251953125);
    lightIntensity = 0.5;
    ambient = 0.800000011920928955078125;
    Flags_FlatColour = 2u;
    Flags_AlphaTested = 4u;
    vec4 _58 = gl_FragCoord;
    _58.w = 1.0 / gl_FragCoord.w;
    FragmentIn param_var_input = FragmentIn(_58, in_var_NORMAL0, in_var_COLOR0, in_var_TEXCOORD0, in_var_COLOR1, in_var_POSITION1);
    vec4 _65 = src_main(param_var_input);
    out_var_SV_Target0 = _65;
}



