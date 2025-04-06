#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform vec4 color;

void main()
{
    vec4 texColor = texture(texture0, texCoord);
    FragColor = mix(color, texColor, texColor.a);
}
