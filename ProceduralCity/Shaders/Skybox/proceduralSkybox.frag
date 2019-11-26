#version 330
in vec3 TexCoords;
out vec4 fragmentColor;

const vec3 topColor = vec3(0,0,0);
const vec3 bottomColor = vec3(0.1,0.2,0.3);
const float topTransientCoord = 0.5;
const float bottomTransientCoord = 0.005;
const int starCutoffLow = 4095;
const int starCutoffHigh = 4096;

float rand(vec3 uv)
{    
	float ret = fract(abs(cos(uv.x * 542.0f + uv.y * 736.0f + uv.z * 247.148f) * 1112.0));
	return ret;
}

float Noise(vec3 uv)
{
	vec3 r = vec3(floor(uv));
	vec3 r1 = vec3(floor(uv) + vec3(1,0,0));
	vec3 r2 = vec3(floor(uv) + vec3(0,1,0));
	vec3 r3 = vec3(floor(uv) + vec3(1,1,0));
	vec3 r4 = vec3(floor(uv) + vec3(0,0,1));
	vec3 r5 = vec3(floor(uv) + vec3(1,0,1));
	vec3 r6 = vec3(floor(uv) + vec3(0,1,1));
	vec3 r7 = vec3(floor(uv) + vec3(1,1,1));
	
	float c = rand(r);
	float c1 = rand(r1);
	float c2 = rand(r2);
	float c3 = rand(r3);
	float c4 = rand(r4);
	float c5 = rand(r5);
	float c6 = rand(r6);
	float c7 = rand(r7);

	float i = mix(c, c1, smoothstep(0.,1.,fract(uv.x)));
	float i1 = mix(c2, c3, smoothstep(0.,1.,fract(uv.x)));
	float i2 = mix(c4, c5, smoothstep(0.,1.,fract(uv.x)));
	float i3 = mix(c6, c7, smoothstep(0.,1.,fract(uv.x)));
	
	float j = mix(i, i1, smoothstep(0.,1.,fract(uv.y)));
	float j1 = mix(i2, i3, smoothstep(0.,1.,fract(uv.y)));

	float k = mix(j, j1, smoothstep(0.,1., fract(uv.z)));
	
	return k;
}

float layeredNoise(vec3 uv)
{
	float z = 0.0;
	z += Noise(uv * 4.0);
	z+= Noise(uv * 8.0) * 0.5;
	z+= Noise(uv * 16.0) * 0.25;
	z+= Noise(uv * 32.0) * 0.125;
	z+= Noise(uv * 64.0) * 0.0625;
	z+= Noise(uv * 128.0) * 0.03125;
	z *= 0.5;
	return z;
}

vec3 clouds(vec3 uv)
{
	vec3 col = vec3(0);
	const float cutOff = 0.6;
	
	float val = layeredNoise(uv);
	float scaledVal = (val - cutOff) / cutOff;

	col = mix(vec3(0, 0, 1) * scaledVal, vec3(0, 1, 0) * scaledVal * 2, scaledVal);
	if(val < cutOff)
	{
		col = vec3(0);
	};
	
	return col;
}

vec3 stars(vec3 uv)
{
	uv = floor(uv * 100000.0f) / 100000.0f;

	int starIntensity = int(rand(uv) * 4096);
	vec3 col = vec3(starIntensity);
	if(starIntensity < starCutoffLow || starIntensity > starCutoffHigh || uv.y < bottomTransientCoord)
	{
		col = vec3(0);
	}

	return col;
}

void main()
{
	vec3 uv = TexCoords;
	const float sphereDistance = 5.0f;
	vec3 sphereUv = normalize(uv) * sphereDistance;  //"project" cube uv-s to a sphere

	vec3 col = vec3(0);
	if(sphereUv.y < bottomTransientCoord)
	{
		col = bottomColor;
	}
	else if(sphereUv.y >= bottomTransientCoord && sphereUv.y < topTransientCoord)
	{
		const float transientLength = topTransientCoord - bottomTransientCoord;
		float scaledTransientCoordinate = (sphereUv.y - bottomTransientCoord) / transientLength;
		col = mix(bottomColor, topColor, smoothstep(0, 1, scaledTransientCoordinate));
	}
	else
	{
		col = topColor;
	}

	col += stars(sphereUv) * 0.0001f;
	col += clouds(sphereUv * 0.25f);

	fragmentColor = vec4(clamp(col, vec3(0), vec3(1)),1.0);
}