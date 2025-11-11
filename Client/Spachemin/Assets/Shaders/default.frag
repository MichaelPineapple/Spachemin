#version 330 core

in vec2 _tex;
in vec3 _normal;
in vec3 _fragPos;

uniform sampler2D texture0;

uniform vec3 uColor;
uniform vec3 uLightAmb;
uniform vec3 uLightDir;
uniform vec3 uLightDirDir;

out vec4 FragColor;

void main()
{
    vec3 norm = normalize(_normal);
    vec3 light = uLightAmb;
    light += max(dot(norm, normalize(-uLightDirDir)), 0.0) * uLightDir;
    vec3 shade = light * uColor;
    FragColor = texture(texture0, _tex) * vec4(shade, 1.0);
}