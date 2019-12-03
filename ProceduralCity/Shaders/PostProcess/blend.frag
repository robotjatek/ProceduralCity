#version 330
in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D u_texture1;
uniform sampler2D u_texture2;

void main()
{
	vec4 texColor1  = texture(u_texture1, fTexCoord);
	vec4 texColor2  = texture(u_texture2, fTexCoord);

	fragmentColor = texColor1 + texColor2;
}