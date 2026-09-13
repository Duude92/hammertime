#version 430
#if defined(GL_EXT_control_flow_attributes)
#extension GL_EXT_control_flow_attributes : require
#define SPIRV_CROSS_FLATTEN [[flatten]]
#define SPIRV_CROSS_BRANCH [[dont_flatten]]
#define SPIRV_CROSS_UNROLL [[unroll]]
#define SPIRV_CROSS_LOOP [[dont_unroll]]
#else
#define SPIRV_CROSS_FLATTEN
#define SPIRV_CROSS_BRANCH
#define SPIRV_CROSS_UNROLL
#define SPIRV_CROSS_LOOP
#endif
layout(points) in;
layout(max_vertices = 4, triangle_strip) out;

struct GeometryIn
{
    vec4 gPosition;
    vec4 gNormal;
    vec4 gColour;
    vec2 gTexture;
    vec4 gTint;
};

struct FragmentIn
{
    vec4 fPosition;
    vec4 fNormal;
    vec4 fColour;
    vec2 fTexture;
    vec4 fTint;
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

layout(binding = 0, std140) uniform type_UVS
{
float FrameCount;
float CurrentFrame;
vec2 UniformPadding;
}
UVS;

layout(location = 0) in
vec4 in_var_NORMAL0[1];
layout(location = 1) in
vec4 in_var_COLOR0[1];
layout(location = 2) in
vec2 in_var_TEXCOORD0[1];
layout(location = 3) in
vec4 in_var_COLOR1[1];
layout(location = 0) out
vec4 out_var_NORMAL0;
layout(location = 1) out
vec4 out_var_COLOR0;
layout(location = 2) out
vec2 out_var_TEXCOORD0;
layout(location = 3) out
vec4 out_var_COLOR1;

mat4 spvWorkaroundRowMajor(mat4 wrap)
{
    return wrap;
}

void src_main(GeometryIn _input[1], FragmentIn _output)
{
    mat4 tModel = transpose(spvWorkaroundRowMajor(Projection.Model));
    mat4 tView = transpose(spvWorkaroundRowMajor(Projection.View));
    mat4 tProjection = transpose(spvWorkaroundRowMajor(Projection.Projection));
    float w = _input[0].gNormal.x / 2.0;
    float h = _input[0].gNormal.y / 2.0;
    vec4 up = vec4(0.0, h, 0.0, 0.0);
    vec4 right = vec4(w, 0.0, 0.0, 0.0);
    vec4 verts[4];
    verts[0] = (-right) + up;
    verts[1] = right + up;
    verts[2] = (-right) - up;
    verts[3] = right - up;
    float column = UVS.CurrentFrame - UVS.FrameCount * trunc(UVS.CurrentFrame / UVS.FrameCount);
    float frameSize = 1.0 / UVS.FrameCount;
    vec2 texCoords[4];
    texCoords[0] = vec2(column * frameSize, 0.0);
    texCoords[1] = vec2((column * frameSize) + frameSize, 0.0);
    texCoords[2] = vec2(column * frameSize, 1.0);
    texCoords[3] = vec2((column * frameSize) + frameSize, 1.0);
    FragmentIn gOut;
    SPIRV_CROSS_UNROLL
    for (int i = 0; i < 4; i++)
    {
        vec4 modelPos = (tModel * verts[i]) + _input[0].gPosition;
        vec4 cameraPos = tView * modelPos;
        vec4 viewportPos = tProjection * cameraPos;
        gOut.fPosition = viewportPos;
        gOut.fNormal = vec4(0.0, 0.0, 0.0, 1.0);
        gOut.fColour = _input[0].gColour;
        gOut.fTexture = texCoords[i];
        gOut.fTint = _input[0].gTint;
        gl_Position = gOut.fPosition;
        out_var_NORMAL0 = gOut.fNormal;
        out_var_COLOR0 = gOut.fColour;
        out_var_TEXCOORD0 = gOut.fTexture;
        out_var_COLOR1 = gOut.fTint;
        EmitVertex();
    }
}

void main()
{
    vec4 _50_unrolled[1];
    for (int i = 0; i < int(1); i++)
    {
        _50_unrolled[i] = gl_in[i].gl_Position;
    }
    GeometryIn param_var_input[1] = GeometryIn[]
    (GeometryIn(_50_unrolled[0], in_var_NORMAL0[0], in_var_COLOR0[0], in_var_TEXCOORD0[0], in_var_COLOR1[0]));
    FragmentIn param_var_output;
    src_main(param_var_input, param_var_output);
    gl_Position.z = 2.0 * gl_Position.z - gl_Position.w;
}

