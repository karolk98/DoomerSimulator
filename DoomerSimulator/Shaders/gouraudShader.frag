#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec4 light;
in float fog;

uniform sampler2D texture0;

void main()
{
    outputColor = light * texture(texture0, texCoord);
    outputColor = mix(outputColor, vec4(0.3f, 0.3f, 0.3f, 1.0f),1-fog);
}