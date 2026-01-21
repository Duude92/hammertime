#version 430

struct VertexIn
{
    vec3 Position;
    vec3 Normal;
    vec4 Colour;
    vec2 Texture;
    vec4 Tint;
    uint Flags;
};

struct PS_INPUT
{
    vec4 Position;
    vec3 TexCoord;
};

layout(binding = 0, std140) uniform type_Projection
{
    layout(row_major)
mat4 Selective;
    layout(row_major)
mat4 Model;
    layout(row_major)
mat4 View;
    layout(row_major)
mat4 Projection;
}
Projection;

layout(location = 0) in
vec3 in_var_POSITION0;
layout(location = 1) in
vec3 in_var_NORMAL0;
layout(location = 2) in
vec4 in_var_COLOR0;
layout(location = 3) in
vec2 in_var_TEXCOORD0;
layout(location = 4) in
vec4 in_var_COLOR1;
layout(location = 5) in
uint in_var_POSITION1;
layout(location = 0) out
vec3 out_var_TEXCOORD0;

mat4 spvWorkaroundRowMajor(mat4 wrap)
{
    return wrap;
}

mat4 _inverse(mat4 m)
{
    float n11 = m[0u].x;
    float n12 = m[1u].x;
    float n13 = m[2u].x;
    float n14 = m[3u].x;
    float n21 = m[0u].y;
    float n22 = m[1u].y;
    float n23 = m[2u].y;
    float n24 = m[3u].y;
    float n31 = m[0u].z;
    float n32 = m[1u].z;
    float n33 = m[2u].z;
    float n34 = m[3u].z;
    float n41 = m[0u].w;
    float n42 = m[1u].w;
    float n43 = m[2u].w;
    float n44 = m[3u].w;
    float t11 = ((((((n23 * n34) * n42) - ((n24 * n33) * n42)) + ((n24 * n32) * n43)) - ((n22 * n34) * n43)) - ((n23 * n32) * n44)) + ((n22 * n33) * n44);
    float t12 = ((((((n14 * n33) * n42) - ((n13 * n34) * n42)) - ((n14 * n32) * n43)) + ((n12 * n34) * n43)) + ((n13 * n32) * n44)) - ((n12 * n33) * n44);
    float t13 = ((((((n13 * n24) * n42) - ((n14 * n23) * n42)) + ((n14 * n22) * n43)) - ((n12 * n24) * n43)) - ((n13 * n22) * n44)) + ((n12 * n23) * n44);
    float t14 = ((((((n14 * n23) * n32) - ((n13 * n24) * n32)) - ((n14 * n22) * n33)) + ((n12 * n24) * n33)) + ((n13 * n22) * n34)) - ((n12 * n23) * n34);
    float det = (((n11 * t11) + (n21 * t12)) + (n31 * t13)) + (n41 * t14);
    float idet = 1.0 / det;
    mat4 ret;
    ret[0u].x = t11 * idet;
    ret[0u].y = (((((((n24 * n33) * n41) - ((n23 * n34) * n41)) - ((n24 * n31) * n43)) + ((n21 * n34) * n43)) + ((n23 * n31) * n44)) - ((n21 * n33) * n44)) * idet;
    ret[0u].z = (((((((n22 * n34) * n41) - ((n24 * n32) * n41)) + ((n24 * n31) * n42)) - ((n21 * n34) * n42)) - ((n22 * n31) * n44)) + ((n21 * n32) * n44)) * idet;
    ret[0u].w = (((((((n23 * n32) * n41) - ((n22 * n33) * n41)) - ((n23 * n31) * n42)) + ((n21 * n33) * n42)) + ((n22 * n31) * n43)) - ((n21 * n32) * n43)) * idet;
    ret[1u].x = t12 * idet;
    ret[1u].y = (((((((n13 * n34) * n41) - ((n14 * n33) * n41)) + ((n14 * n31) * n43)) - ((n11 * n34) * n43)) - ((n13 * n31) * n44)) + ((n11 * n33) * n44)) * idet;
    ret[1u].z = (((((((n14 * n32) * n41) - ((n12 * n34) * n41)) - ((n14 * n31) * n42)) + ((n11 * n34) * n42)) + ((n12 * n31) * n44)) - ((n11 * n32) * n44)) * idet;
    ret[1u].w = (((((((n12 * n33) * n41) - ((n13 * n32) * n41)) + ((n13 * n31) * n42)) - ((n11 * n33) * n42)) - ((n12 * n31) * n43)) + ((n11 * n32) * n43)) * idet;
    ret[2u].x = t13 * idet;
    ret[2u].y = (((((((n14 * n23) * n41) - ((n13 * n24) * n41)) - ((n14 * n21) * n43)) + ((n11 * n24) * n43)) + ((n13 * n21) * n44)) - ((n11 * n23) * n44)) * idet;
    ret[2u].z = (((((((n12 * n24) * n41) - ((n14 * n22) * n41)) + ((n14 * n21) * n42)) - ((n11 * n24) * n42)) - ((n12 * n21) * n44)) + ((n11 * n22) * n44)) * idet;
    ret[2u].w = (((((((n13 * n22) * n41) - ((n12 * n23) * n41)) - ((n13 * n21) * n42)) + ((n11 * n23) * n42)) + ((n12 * n21) * n43)) - ((n11 * n22) * n43)) * idet;
    ret[3u].x = t14 * idet;
    ret[3u].y = (((((((n13 * n24) * n31) - ((n14 * n23) * n31)) + ((n14 * n21) * n33)) - ((n11 * n24) * n33)) - ((n13 * n21) * n34)) + ((n11 * n23) * n34)) * idet;
    ret[3u].z = (((((((n14 * n22) * n31) - ((n12 * n24) * n31)) - ((n14 * n21) * n32)) + ((n11 * n24) * n32)) + ((n12 * n21) * n34)) - ((n11 * n22) * n34)) * idet;
    ret[3u].w = (((((((n12 * n23) * n31) - ((n13 * n22) * n31)) + ((n13 * n21) * n32)) - ((n11 * n23) * n32)) - ((n12 * n21) * n33)) + ((n11 * n22) * n33)) * idet;
    return ret;
}

PS_INPUT src_main(VertexIn _input)
{
    mat4 param_var_m = spvWorkaroundRowMajor(Projection.View);
    PS_INPUT _output;
    _output.Position = ((spvWorkaroundRowMajor(Projection.Projection) * _inverse(param_var_m)) * vec4(_input.Position, 1.0)).xyww;
    _output.TexCoord = _input.Position;
    return _output;
}

void main()
{
    VertexIn param_var_input = VertexIn(in_var_POSITION0, in_var_NORMAL0, in_var_COLOR0, in_var_TEXCOORD0, in_var_COLOR1, in_var_POSITION1);
    PS_INPUT _49 = src_main(param_var_input);
    gl_Position = _49.Position;
    out_var_TEXCOORD0 = _49.TexCoord;
    gl_Position.z = 2.0 * gl_Position.z - gl_Position.w;
}

