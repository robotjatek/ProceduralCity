#version 330

// Dependencies: exponentialFog definition

// A shader for untextured meshes. Needs a color value provided as a uniform.
// Flat colored meshes are affected by the value too. Should be linked with a shader containing the definition of the exponential fog function.

in vec2 texCoords;
out vec4 fragColor;

uniform vec3 u_color;
uniform vec3 fogColor;

// Forward declaration of the exponential fog function. This shader file should be linked with a shader containing the definintion
float exponentialFog();

void main()
{
	float fogFactor = exponentialFog();
	fragColor = mix(vec4(fogColor, 1.0), vec4(u_color, 1), fogFactor);
}