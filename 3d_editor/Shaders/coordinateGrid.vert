#version 330 core
in vec3 aPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 FragPos;

void main()
{
	FragPos = vec3(vec4(aPos, 1.0) * model);
	gl_Position = vec4(aPos, 1.0) * model * view * projection;
}