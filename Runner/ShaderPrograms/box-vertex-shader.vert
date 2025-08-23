#version 330 core

uniform vec2 ViewportSize;
uniform float ColorFactor;

layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aColor;

out vec4 vColor;

void main()
{
    float nx = aPosition.x / ViewportSize.x * 2f - 1f;
    float ny = aPosition.y / ViewportSize.y * 2f - 1f;
    gl_Position = vec4(nx, ny, 0f, 1f);

    vColor = aColor * ColorFactor;
}