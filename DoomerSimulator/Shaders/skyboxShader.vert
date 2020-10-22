#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;

out vec2 texCoord;
out vec3 FragPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    texCoord = aTexCoord;
    FragPos = vec3(vec4(aPosition, 1.0)*model);
    
    gl_Position = vec4(aPosition, 1.0)*model*view*projection;
}