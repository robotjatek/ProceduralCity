#version 330
layout(location = 0) in vec3 vVertex;
layout(location = 2) in vec2 vUV;

out vec2 fUV;

uniform mat4 _projection;
uniform mat4 _view;
uniform mat4 _model;

void main()
{
    fUV = vUV;
    gl_Position = _projection * _view * _model * vec4(vVertex, 1.0f);
}