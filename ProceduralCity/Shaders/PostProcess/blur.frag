#version 330
in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D u_texture;
uniform vec2 u_Offset;

void main()
{
	vec4 texColor  = vec4(0);
	texColor += texture(u_texture, fTexCoord - u_Offset * 2) * 0.1784;
	texColor += texture(u_texture, fTexCoord - u_Offset) * 0.210431;
	texColor += texture(u_texture, fTexCoord) * 0.222338;
	texColor += texture(u_texture, fTexCoord + u_Offset) * 0.210431;
	texColor += texture(u_texture, fTexCoord + u_Offset * 2) * 0.1784;

	fragmentColor = texColor;
}