#version 330
in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D tex;
uniform float u_LuminanceTreshold;

void main()
{
	const vec3 luminanceConstants = vec3(0.2126, 0.7152, 0.0722);
	vec4 color = texture(tex, fTexCoord);
	vec3 rgb = color.rgb;
	float luminance = dot(rgb, luminanceConstants);

	if(luminance < u_LuminanceTreshold)
	{
		fragmentColor = vec4(0);
	}
	else
	{
		fragmentColor = texture(tex, fTexCoord);
	}
}