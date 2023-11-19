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
		fragmentColor = vec4(0); // Postprocess pipeline is additive. Returning 0 essentially leaves the pixel unchanged.
	}
	else
	{
		fragmentColor = texture(tex, fTexCoord) * clamp(0, 1, smoothstep(0, .4f, 1.0f/u_LuminanceTreshold));
	}
}