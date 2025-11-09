#version 330 core

in vec2 _tex;
in vec3 _normal;
in vec3 _fragPos;

uniform sampler2D texture0;
uniform vec3 color;
uniform vec3 lightAmb;
uniform vec3 lightDirDir;
uniform vec3 lightDirColor;

out vec4 FragColor;

void main()
{
    vec3 norm = normalize(_normal);
    vec3 light = lightAmb;
    light += max(dot(norm, normalize(-lightDirDir)), 0.0) * lightDirColor;
    vec3 shade = light * color;
    FragColor = texture(texture0, _tex) * vec4(shade, 1.0);
}