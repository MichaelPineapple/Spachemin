#version 330 core

in vec2 _tex;
in vec3 _normal;
in vec3 _fragPos;

uniform sampler2D texture0;
uniform vec3 color;
uniform vec3 lightAmb;

out vec4 FragColor;

void main()
{
    vec3 light = lightAmb;
    vec3 shade = light * color;
    FragColor = texture(texture0, _tex) * vec4(shade, 1.0);
}