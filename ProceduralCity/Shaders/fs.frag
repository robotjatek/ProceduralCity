#version 330
in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D tex;
uniform vec3 fogColor;

// Linear fog
//float near = 0.01f;
//float far = 50.0f;
//
//float linearizeDepth(float depth)
//{
//	return (2.0f * near * far) / (far + near  - (depth * 2.0f - 1.0f) * (far - near));
//}

float exponentialFog();

void main()
{
// Old fog calculation
//	float originalZ = linearizeDepth(gl_FragCoord.z) / far;
//	float fogFactor = 1.0f - originalZ;
//	fragmentColor = mix(vec4(fogColor, 1.0), texture(tex, fTexCoord), vec4(vec3(fogFactor), 1.0));

	// New fog calculation
	float factor = exponentialFog();
	fragmentColor = mix(vec4(fogColor, 1.0), texture(tex, fTexCoord), factor);
}