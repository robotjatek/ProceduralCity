#version 330
in vec2 fTexCoord;
out vec4 fragmentColor;

float rand(vec2 uv)
{    
	float ret = fract(abs(cos(uv.x * 242.0f + uv.y * 337.f) * 2212.0));
	return ret;
}

void main()
{
	vec2 uv = fTexCoord;
	vec3 col = vec3(0);

	col = vec3(rand(uv));
	fragmentColor = vec4(clamp(col, vec3(0), vec3(1)),1.0);
}