#version 330
in vec2 fTexCoord;
in vec3 fVertexData;
out vec4 fragmentColor;

uniform sampler2D tex;
uniform float hue = 1;
uniform float saturation = 0;
uniform float value = 1;
const float gCutoff = 0.1;

vec3 hsvToRgb(float h, float s, float v);

float clip(float alpha, float cutoff)
{
	if(alpha < cutoff)
	{
		discard;
	}

	return alpha;
}

void main()
{
	float alpha = texture(tex, fTexCoord).a;
	fragmentColor = vec4(hsvToRgb(hue, saturation, value), clip(alpha, gCutoff));
}