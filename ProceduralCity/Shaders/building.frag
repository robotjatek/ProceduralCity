#version 330 core

in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D tex; // The input texture is a grayscale R8 texture
uniform vec3 fogColor;
uniform vec3 buildingColor = vec3(1,0,0);

float exponentialFog();

void main()
{
    float intensity = texture(tex, fTexCoord).r;
    vec4 color = intensity * vec4(buildingColor, 1.0);
    float fogFactor = exponentialFog();
    vec4 fogAffectedColor = mix(vec4(fogColor, 1.f), color, fogFactor);

    // High intenity colors are less affected by the fog
    if (intensity > 0.4f) {
        // 0 means the "light" is fully affected by the fog
        // 1 means the "light" is not affected by the fog
        fogAffectedColor = mix(fogAffectedColor, color, 1.f - 0.1 * fogFactor); // The fog has a minuscule effect on high intensity ligths too
    }

    fragmentColor = fogAffectedColor;
}

//void main()
//{
//	float intensity = texture(tex, fTexCoord).r;
//	vec4 color = intensity * vec4(buildingColor, 1.0);
//	float factor = exponentialFog();
//
//	if (intensity <= 0.4f) { // If the window light is not strong enough the fog will fade it.
//		fragmentColor = mix(vec4(fogColor, 1.0), color, factor);
//	} else {
//		fragmentColor = color; // strong enough window "lights" can shine through the fog
//	}
//}