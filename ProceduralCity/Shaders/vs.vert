#version 330
layout(location = 0) in vec3 vVertexData;
//layout(location = 1) in vec3 vNormals;
layout(location = 2) in vec2 vTexCoord;

uniform mat4 _projection;
uniform mat4 _view;
uniform mat4 _model = mat4(1);

out vec2 fTexCoord;

void main()
{
	fTexCoord = vTexCoord;
	gl_Position = _projection* _view * _model * vec4(vVertexData, 1.0f);

}