#version 330
layout(location = 0) in vec3 vVertexData;
layout(location = 2) in vec2 vTexCoord;

layout(location = 3) in mat4 _model;

uniform mat4 _projection;
uniform mat4 _view;

out vec2 fTexCoord;

void main()
{
	fTexCoord = vTexCoord;
	gl_Position = _projection* _view * _model * vec4(vVertexData, 1.0f);

}