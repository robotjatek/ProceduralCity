#version 330
in vec2 fUV;
out vec4 colorData;

uniform vec2 center = vec2(0.5, 0.5);
const float radius = 0.3;

void main()
{
    float fragmentDistance = distance(fUV, center);
    if(fragmentDistance > radius)
    {
        discard; //TODO: test if alpha blending gives better result
    }

    float multiplier = 1.0 - fragmentDistance/radius;

    colorData = vec4(multiplier, 0, 0, 1);
    //colorData = vec4(1, 0, 0, 1);
}