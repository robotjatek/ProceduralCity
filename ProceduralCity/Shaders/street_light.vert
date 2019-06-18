#version 330
layout(location = 0) in vec3 vVertex;
layout(location = 2) in vec2 vUV;

out vec2 fUV;

uniform mat4 MVP;

void main()
{
    fUV = vUV;
    gl_Position = MVP * vec4(vVertex, 1.0f);
}