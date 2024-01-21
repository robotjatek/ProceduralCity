#version 330 core

in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D tex;
uniform vec3 fogColor;
uniform vec3 buildingColor = vec3(1,0,0);

float exponentialFog();

void main()
{
	float factor = exponentialFog();
	vec4 color = texture(tex, fTexCoord) * vec4(buildingColor, 1.0);
	fragmentColor = mix(vec4(fogColor, 1.0), color, factor);
}