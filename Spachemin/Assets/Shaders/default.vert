#version 330 core

in vec3 aVert;
in vec2 aTex;
in vec3 aNormal;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec2 _tex;
out vec3 _normal;
out vec3 _fragPos;

void main()
{
    gl_Position = vec4(aVert, 1.0) * uModel * uView * uProjection;
    _fragPos = vec3(vec4(aVert, 1.0) * uModel);
    _tex = aTex;
    _normal = aNormal * mat3(transpose(inverse(uModel)));;
}