#version 330 core
out vec4 FragColor;

in vec3 texCoord;

uniform samplerCube cubeSampler;
uniform vec4 color;

void main()
{
    vec4 texColor = texture(cubeSampler, texCoord);
    FragColor = mix(color, texColor, texColor.a);
}
