#version 330 core
in vec3 aPos;
in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 transform;

void main()
{
	gl_Position = vec4(aPos, 1.0) * transform;
	texCoord = aTexCoord;
}