#version 330
in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D u_texture;
uniform vec2 u_Offset;

void main()
{
	vec4 texColor  = vec4(0);
	texColor += texture(u_texture, fTexCoord - u_Offset * 5) * 0.066414f;
	texColor += texture(u_texture, fTexCoord - u_Offset * 4) * 0.079465f;
	texColor += texture(u_texture, fTexCoord - u_Offset * 3) * 0.091364f;
	texColor += texture(u_texture, fTexCoord - u_Offset * 2) * 0.100939f;
	texColor += texture(u_texture, fTexCoord - u_Offset) * 0.107159f;

	texColor += texture(u_texture, fTexCoord) * 0.109317f;

	texColor += texture(u_texture, fTexCoord + u_Offset) * 0.107159f;
	texColor += texture(u_texture, fTexCoord + u_Offset * 2) * 0.100939f;
	texColor += texture(u_texture, fTexCoord + u_Offset * 3) * 0.091364f;
	texColor += texture(u_texture, fTexCoord + u_Offset * 4) * 0.079465f;
	texColor += texture(u_texture, fTexCoord + u_Offset * 5) * 0.066414f;

	fragmentColor = texColor;
}