#version 330
in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D tex;
uniform float fadeFactor = 1.0f;

void main()
{
	fragmentColor = texture(tex, fTexCoord) * fadeFactor;
}