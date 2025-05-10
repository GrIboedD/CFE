#version 330 core

in vec3 FragPos;
out vec4 FragColor;

uniform vec4 color ;
uniform float gridStep;
uniform float lineWidth;

uniform vec3 cameraPos;

uniform float fogDistance;
uniform float fogFactor;

void main()
{
	float distanseFromView = distance(cameraPos, FragPos);

	float beta = 1.0;

	if (distanseFromView >= fogDistance)
	{
		beta = exp((fogDistance-distanseFromView) * fogFactor);
	}


	vec2 distanceToNearestLine = 1.0 - abs((fract(abs(FragPos.xz) / gridStep) - 0.5)) * 2.0;
	float relativeFragMinCord = min(distanceToNearestLine.x, distanceToNearestLine.y);

	float relativeWidth = lineWidth / gridStep;

	float alpha = 0;

    if (relativeFragMinCord >= relativeWidth)
    {
        discard;
    }




	FragColor = color * beta;

}