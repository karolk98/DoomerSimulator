#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec3 FragPos;

uniform sampler2D texture0;
uniform sampler2D texture1;
uniform float time;
uniform float fogStrength;

void main()
{
    outputColor = mix(mix(texture(texture0, texCoord), texture(texture1, texCoord), time), vec4(0.5f, 0.5f, 0.5f, 1.0f),1-fogStrength);
}