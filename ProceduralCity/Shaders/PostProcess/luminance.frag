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
		/*
		 * To calculate bloom, the postprocess effect is blended together with the original input picture.
		 * So by returning 0 here, the resulting pixel will essentially be the original pixel
		 */
		fragmentColor = vec4(0); 
	}
	else
	{
		fragmentColor = texture(tex, fTexCoord) * clamp(0, 1, smoothstep(0, .4f, 1.0f/u_LuminanceTreshold));
	}

	// A second option is just to multiply the luminance and the pixels
	//fragmentColor = texture(tex, fTexCoord) * luminance;
}