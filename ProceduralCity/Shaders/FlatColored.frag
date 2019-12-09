﻿#version 330

in vec2 texCoords;
out vec4 fragColor;

uniform vec3 u_color;

void main()
{
	fragColor = vec4(u_color, 1);
}