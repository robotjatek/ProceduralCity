#version 330
in vec3 TexCoords;
out vec4 fragmentColor;

uniform samplerCube skybox;

void main()
{
	fragmentColor = texture(skybox, TexCoords)*0.15f;
}