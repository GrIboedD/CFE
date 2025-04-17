#version 330 core
in vec3 aPos;
in vec3 aNormal;

out vec3 Normal;
out vec3 FragPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform mat3 normalMatrix;

void main()
{
	Normal = aNormal * normalMatrix;
	FragPos = vec3(vec4(aPos, 1.0) * model);
	gl_Position = vec4(aPos, 1.0) * model * view * projection; 
}