#version 330 core

in vec3 FragPos;
out vec4 FragColor;

uniform vec4 color;
uniform float gridStep;
uniform float lineWidth;

uniform vec3 cameraPos;

void main()
{
	float distanseFromView = distance(vec3(0.0), FragPos);

	float lod = clamp(1, 10, distanseFromView * 0.1);

	float aStep = gridStep;

	if (distanseFromView >= 50)
	{
		gridStep * (1+lod);
	}


	vec2 distanceToNearestLine = 1.0 - abs((fract(abs(FragPos.xz) / aStep) - 0.5)) * 2.0;
	float relativeFragMinCord = min(distanceToNearestLine.x, distanceToNearestLine.y);

	float relativeWidth = lineWidth / aStep;

	float alpha = 0;

	if (relativeFragMinCord < relativeWidth)
	{
	alpha =1;
	}




	FragColor = color * alpha;

}