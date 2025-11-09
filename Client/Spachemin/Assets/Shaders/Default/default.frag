#version 330 core

in vec2 _tex;
in vec3 _normal;
in vec3 _fragPos;

uniform sampler2D texture0;
uniform vec3 color;

out vec4 FragColor;

void main()
{
    FragColor = texture(texture0, _tex) * vec4(color, 1.0);
}