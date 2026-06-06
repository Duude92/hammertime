#version 430

layout(binding = 0, std140) uniform ProjectionMatrixBuffer
{
    mat4 ProjectionMatrix;
};

layout(location = 0) in vec2 in_pos;
layout(location = 1) in vec2 in_uv;
layout(location = 2) in vec4 in_col;

layout(location = 0) out vec4 frag_col;
layout(location = 1) out vec2 frag_uv;

void main()
{
    vec2 pos = in_pos;

    gl_Position = ProjectionMatrix * vec4(pos, 0.0, 1.0);

    frag_col = in_col;
    frag_uv  = in_uv;
}
