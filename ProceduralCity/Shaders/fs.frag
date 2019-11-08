#version 330
in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D tex;

void main()
{
	fragmentColor = texture(tex, fTexCoord);
}