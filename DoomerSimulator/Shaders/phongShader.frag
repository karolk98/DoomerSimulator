#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec3 Normal;
in vec3 FragPos;

struct Spotlight {
    vec3  position;
    vec3  direction;
    float cutOff;
};

uniform sampler2D texture0;
uniform vec3 lightPos;
uniform vec3 lightColor;
uniform vec3 viewPos;
uniform int lightingModel;
uniform float time;
uniform Spotlight spotlight;
uniform int fogStrength;

void main()
{
    float ambientStrength = 0.1;
    float specularStrength = 0.5;
    
    vec3 ambient = ambientStrength * lightColor;
    
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;
    vec3 specular;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir;
    if(lightingModel==1)
    {
        reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
        specular = specularStrength * spec * lightColor;
    }
    else
    {
        vec3 H = normalize(viewDir+lightDir);
        float spec = max(dot(norm, H), 0.0);
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

    float fog = min(fogStrength/dot(FragPos - viewPos,FragPos - viewPos),1);
    
    outputColor = vec4((ambient + time*(diffuse + specular))+ spot,1.0) * texture(texture0, texCoord);
    outputColor = mix(outputColor, vec4(0.3f, 0.3f, 0.3f, 1.0f),1-fog);
}