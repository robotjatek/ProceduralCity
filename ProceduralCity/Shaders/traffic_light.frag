#version 330
in vec2 fTexCoord;
out vec4 colorData;

const vec3 headlightColor = vec3(1.f, 0.945f, 0.878f);
const vec3 backlightColor = vec3(0.9f, 0.2f, 0.1f);
const vec2 center = vec2(0.5,0.5);


void main()
{
    vec3 lightColor = backlightColor;
    if (gl_FrontFacing)
    {
        lightColor = headlightColor;
    }

    float fragmentDistance = distance(fTexCoord, center);
    if(fragmentDistance > 0.5)
    {
        discard;
    }

    colorData = vec4(lightColor * (1 - fragmentDistance), 1);
}