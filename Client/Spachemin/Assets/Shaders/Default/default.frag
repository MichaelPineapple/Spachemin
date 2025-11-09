#version 330 core

in vec2 _tex;
in vec3 _normal;
in vec3 _fragPos;

uniform vec3 color;

out vec4 FragColor;

void main()
{
    FragColor = vec4(color, 1.0);
}