#version 330 core
out vec4 FragColor;

in vec3 texCoord;
in vec3 Normal;
in vec3 FragPos;

uniform samplerCube cubeSampler;
uniform vec4 color;

uniform vec3 lightsColor;
uniform vec3 lightsPosition;
uniform float ambientStrength;

void main()
{
    vec3 ambient = ambientStrength * lightsColor;

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightsPosition - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightsColor;




    vec4 texColor = texture(cubeSampler, texCoord);
    FragColor = mix(color, texColor, texColor.a) * vec4(diffuse + ambient, 1.0);
}
