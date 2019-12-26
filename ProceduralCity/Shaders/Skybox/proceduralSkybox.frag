#version 330
in vec3 TexCoords;
out vec4 fragmentColor;

uniform float u_seed_x = 542.0f;
uniform float u_seed_y = 736.0f;
uniform float u_seed_z = 247.147f;
uniform float u_seed_scale = 1112.0f;

uniform vec3 u_cloud_color_1 = vec3(0,0,1);
uniform vec3 u_cloud_color_2 = vec3(0,1,0);
uniform float u_cloud_cutoff = 0.65f;
uniform vec3 u_sky_top_color = vec3(0,0,0);
uniform vec3 u_sky_bottom_color = vec3(0.1,0.2,0.3);
const float topTransientCoord = 0.01f;
const float bottomTransientCoord = -0.5;
const int starCutoffLow = 4095;
const int starCutoffHigh = 4096;

float rand(vec3 uv)
{    
	return fract(abs(cos(uv.x * u_seed_x + uv.y * u_seed_y + uv.z * u_seed_z) * u_seed_scale));
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
	float val = layeredNoise(uv);
	float scaledVal = (val - u_cloud_cutoff) / u_cloud_cutoff;

	col = mix(u_cloud_color_1 * scaledVal, u_cloud_color_2 * scaledVal * 2, scaledVal);
	if(val < u_cloud_cutoff)
	{
		col = vec3(0);
	};
	
	return col;
}

vec3 stars(vec3 uv)
{
	uv = floor(uv * 1000.0f) / 1000.0f;

	int starIntensity = int(rand(uv) * 4096);
	vec3 col = vec3(starIntensity);
	if(starIntensity < starCutoffLow || starIntensity > starCutoffHigh)
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

	float intensityScale = 1.0f;
	vec3 col = vec3(0);
	if(sphereUv.y < bottomTransientCoord)
	{
		col = u_sky_bottom_color;
		intensityScale = 0.0f;
	}
	else if(sphereUv.y >= bottomTransientCoord && sphereUv.y < topTransientCoord)
	{
		const float transientLength = topTransientCoord - bottomTransientCoord;
		float scaledTransientCoordinate = (sphereUv.y - bottomTransientCoord) / transientLength;
		col = mix(u_sky_bottom_color, u_sky_top_color, smoothstep(0, 1, scaledTransientCoordinate));
		intensityScale = mix(0, 1, smoothstep(0, 1, scaledTransientCoordinate));
	}
	else
	{
		col = u_sky_top_color;
	}

	col += stars(sphereUv) * 0.00015f * intensityScale;
	col += clouds(sphereUv * 0.25f) * intensityScale;

	fragmentColor = vec4(clamp(col, vec3(0), vec3(1)),1.0);
}