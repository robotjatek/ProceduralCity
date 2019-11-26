#version 330
layout(location = 0) in vec3 position;
out vec3 TexCoords;

uniform mat4 _projection;
uniform mat4 _view;

void main()
{
	TexCoords = vec3(position);
	vec4 pos = _projection * _view * vec4(position, 1.0f);
	gl_Position = pos.xyww;
}