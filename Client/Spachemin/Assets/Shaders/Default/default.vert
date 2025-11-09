#version 330 core

in vec3 aVert;
in vec2 aTex;
in vec3 aColor;
in vec3 aNormal;

uniform mat4 model;
uniform mat4 vieww;
uniform mat4 projj;

out vec2 _tex;
out vec3 _normal;
out vec3 _fragPos;

void main()
{
    gl_Position = vec4(aVert, 1.0) * model * vieww * projj;
    _fragPos = vec3(vec4(aVert, 1.0) * model);
    _tex = aTex;
    _normal = aNormal * mat3(transpose(inverse(model)));;
}