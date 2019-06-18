#version 330
layout(location = 1) in vec3 position;
out vec3 TexCoords;

uniform mat4 projection;
uniform mat4 view;

void main()
{
	vec4 pos = projection*view*vec4(position, 1.0f);
	TexCoords = position;
	gl_Position = pos.xyww;
}