#version 330
//in vec3 fNormals;
in vec2 fTexCoord;
in vec3 fVertexData;
out vec4 fragmentColor;

uniform sampler2D tex;

void main()
{
	fragmentColor = texture(tex, fTexCoord);
	//fragmentColor = vec4(fTexCoord,0,1);
	//fragmentColor = vec4(fVertexData,1);
}