#version 330 core
out vec4 FragColor;

in vec3 texCoord;
in vec3 Normal;
in vec3 FragPos;

uniform samplerCube cubeSampler;

uniform vec3 viewPos;

uniform bool usePerpendicularCull = false;
uniform vec3 sphereCenter = vec3(0, 0, 0);

struct Material {
    vec4 color;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
};

struct Light {
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Material material;
uniform Light light;


void main()
{
    vec3 ambient = light.ambient * material.ambient;

    vec3 norm = normalize(Normal);

        if (usePerpendicularCull) {
        vec3 camToCenter = normalize(sphereCenter - viewPos);
        float dotProd = dot(norm, camToCenter);

        float threshold = 0.35;

        if (abs(dotProd) >= threshold) {
            discard;
        }
    }

    vec3 lightDir = normalize(light.direction);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * light.diffuse * material.diffuse;

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = material.specular * spec * light.specular;


    vec4 texColor = texture(cubeSampler, texCoord);
    FragColor = mix(material.color, texColor, texColor.a) * vec4(ambient + diffuse, 1.0) + vec4(specular, 1.0);
}
