#version 330

// Contains the definition for the exponential fog. Not a standalone shader.

uniform float fogDensity = 4.0f;
float exponentialFog()
{
	float z = (gl_FragCoord.z / gl_FragCoord.w) / 5000.0;
	float factor = exp(-pow(z * fogDensity, 2.0));
	return clamp(0, 1, factor);
}