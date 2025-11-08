#version 330 core

in vec2 vert;

uniform mat4 model;
uniform mat4 projj;

void main()
{
    gl_Position = vec4(vert, 0.0, 1.0) * model * projj;
}