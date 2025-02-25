/*
#version 450

layout(location = 0) in
vec3 Position;

layout(set = 0, binding = 0) uniform LightViewProjectionBuffer {
mat4 LightViewProj;
};

layout(set = 1, binding = 0) uniform WorldBuffer {
mat4 World;
};

layout(location = 0) out
vec4 fsin_LightSpacePos;
*/
/*
cbuffer Projection
{
    matrix Selective;
    matrix Model;
    matrix View;
    matrix Projection;
};

*/
/*
struct VertexIn
{
    float3 Position : POSITION0;
//    float3 Normal : NORMAL0;
//    float4 Colour : COLOR0;
//    float2 Texture : TEXCOORD0;
//    float4 Tint : COLOR1;
//    uint1 Flags : POSITION1;
};
*/

struct PS_INPUT
{
    float4 Position : SV_Position;
    float2 uv : TEXCOORD0;
};
float4 vertices[3] =
{
    float4(10000.0, -10000.0, 0.0, 1.0), // Bottom-left
    float4(-10000.0, -10000.0, 0.0, 1.0), // Bottom-right
    float4(0.0, 10000.0, 0.0, 1.0) // Top
};


PS_INPUT main(uint vertexID : SV_VertexID)
{
    float2 uv = float2((vertexID << 1) & 2, vertexID & 2);
    //float4 pos = float4(uv * 2.0 - 1.0, 0.0, 1.0);
    float4 pos = vertices[vertexID];
    
    PS_INPUT output;
    output.Position = pos;
    output.uv = uv;
    return output;
}