#version 330
in vec2 fTexCoord;
out vec4 colorData;

uniform vec3 u_color;
const vec2 center = vec2(0.5,0.5);

void main()
{
    float fragmentDistance = distance(fTexCoord, center);

    if(fragmentDistance > 0.5)
    {
        discard;
    }

    colorData = vec4(u_color * (1 - fragmentDistance), 1);
}