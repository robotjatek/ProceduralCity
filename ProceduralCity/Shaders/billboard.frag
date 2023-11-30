#version 330

in vec2 fTexCoord;
out vec4 fragmentColor;

uniform sampler2D tex;
uniform vec3 billboardColor = vec3(1, 0, 1); // HSV

vec3 hsvToRgb(float h, float s, float v);

void main()
{
	fragmentColor = texture(tex, fTexCoord) * vec4(hsvToRgb(billboardColor.x, billboardColor.y, billboardColor.z), 1);
}