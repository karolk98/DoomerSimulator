#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec3 aNormal;

out vec2 texCoord;
out vec4 light;
out float fog;

struct Spotlight {
    vec3  position;
    vec3  direction;
    float cutOff;
};

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 lightPos;
uniform vec3 lightColor;
uniform vec3 viewPos;
uniform int lightingModel;
uniform float time;
uniform Spotlight spotlight;
uniform int fogStrength;

void main(void)
{
    texCoord = aTexCoord;
    vec3 Normal = aNormal * mat3(transpose(inverse(model)));
    vec3 FragPos = vec3(vec4(aPosition, 1.0)*model);

    float ambientStrength = 0.1;
    float specularStrength = 0.5;

    vec3 ambient = ambientStrength * lightColor;

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;
    vec3 specular;
    vec3 viewDir = normalize(viewPos - FragPos);
    if(lightingModel==1)
    {
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
        specular = specularStrength * spec * lightColor;
    }
    else
    {
        vec3 H = normalize(viewDir+lightDir);
        float spec = max(dot(norm, lightDir), 0.0);
        specular = specularStrength * spec * lightColor;
    }

    lightDir = normalize(spotlight.position - FragPos);
    float theta     = dot(lightDir, normalize(-spotlight.direction));
    vec3 spot;
    if(theta > spotlight.cutOff)
    {
        vec3 spotlightToFragment = normalize(FragPos - spotlight.position);
        float spotlightVariation = max(dot(spotlightToFragment, spotlight.direction), 0.0);
        spotlightVariation = pow(spotlightVariation, 15);
        float spotightIntensity = min(100/dot(FragPos - spotlight.position, FragPos - spotlight.position), 1);
        spot = spotlightVariation * lightColor * spotightIntensity;
    }
    
    light = vec4((ambient + time*(diffuse + specular)+spot),1.0);
    gl_Position = vec4(aPosition, 1.0)*model*view*projection;

    fog = min(fogStrength/dot(FragPos - viewPos,FragPos - viewPos),1);
}