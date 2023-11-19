#version 330

// Not a standalone shader.

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