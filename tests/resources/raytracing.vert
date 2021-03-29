#version 420

layout(location = 0) in vec2 pos;
layout(location = 1) in vec2 tex;

out vec2 texCoord;

void main() {
	texCoord = tex;
	gl_Position = vec4(pos.x, -pos.y, 0, 1.0);
}