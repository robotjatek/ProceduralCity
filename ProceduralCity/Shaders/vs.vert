#version 330
layout(location = 0) in vec3 vVertexData;
//layout(location = 1) in vec3 vNormals;
layout(location = 2) in vec2 vTexCoord;

uniform mat4 MVP;
//out fNormals;
out vec2 fTexCoord;
out vec3 fVertexData;

void main()
{
	//fNormals = vNormals;
	fVertexData = vVertexData;
	fTexCoord = vTexCoord;
	gl_Position = MVP*vec4(vVertexData, 1.0f);
}