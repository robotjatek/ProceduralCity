#version 330
in vec2 fTexCoord;
in vec3 fVertexData;
out vec4 fragmentColor;

uniform sampler2D tex;
uniform float hue = 1;
uniform float saturation = 0;
uniform float value = 1;
const float gCutoff = 0.1;

//HSV to RGB: http://www.chilliant.com/rgb2hsv.html
#define saturate(x) clamp(x, 0., 1.)
vec3 hueToRgb(float h)
{
	float r = abs(h * 6 - 3) - 1;
	float g = 2 - abs(h * 6 - 2);
	float b = 2 - abs(h * 6 - 4);
	return saturate(vec3(r,g,b));
}

vec3 hsvToRgb(float h, float s, float v)
{
	vec3 rgb = hueToRgb(h);
	return ((rgb - 1) * s + 1) * v;
}

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