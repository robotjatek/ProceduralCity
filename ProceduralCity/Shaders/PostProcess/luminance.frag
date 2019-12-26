#version 330
in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D tex;
uniform float u_LuminanceTreshold;

void main()
{
	const vec3 luminanceConstants = vec3(0.2126f, 0.7152f, 0.0722f);
	vec4 color = texture(tex, fTexCoord);
	vec3 rgb = color.rgb;
	float luminance = dot(rgb, luminanceConstants);

	if(luminance < u_LuminanceTreshold)
	{
		fragmentColor = vec4(0);
	}
	else
	{
		fragmentColor = texture(tex, fTexCoord) * mix(0, 1, 1.0f/u_LuminanceTreshold);
	}
}