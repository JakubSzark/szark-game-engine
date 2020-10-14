#include "SzarkCore.h"

static bool s_rendererInitialized = false;

static uint s_defaultQuadEBO;

static const float s_quadVertexData[] = {
	// Pos   | Coords
	 1.0, 1.0, 1.0, 1.0,
	 1.0,-1.0, 1.0, 0.0,
	-1.0,-1.0, 0.0, 0.0,
	-1.0, 1.0, 0.0, 1.0
};

static const uint s_quadIndices[] = {
	0, 1, 3, 1, 2, 3
};

/* Creates a render texture */
auto GenerateTextureID(Color* pixels, uint width, uint height) -> uint {
	if (width == 0 || height == 0 || pixels == nullptr) {
		return 0;
	}

	uint id = 0;

	glGenTextures(1, &id);
	glBindTexture(GL_TEXTURE_2D, id);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width,
		height, 0, GL_RGB, GL_UNSIGNED_BYTE, pixels);

	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);

	return id;
}

/* Updates a texture on the graphics card */
auto inline UpdateTexture(uint id, Color* pixels, uint width, uint height) -> void {
	glBindTexture(GL_TEXTURE_2D, id);
	glTexSubImage2D(GL_TEXTURE_2D, 0, 0, 0, width, height,
		GL_RGB, GL_UNSIGNED_BYTE, pixels);
}

/* Uses a texture */
auto inline UseTexture(uint id) -> void {
	glBindTexture(GL_TEXTURE_2D, id);
}

/* Renders a quad on screen */
auto RenderQuad() -> void {
	// Renderer must be initialized to render a quad
	if (!s_rendererInitialized) {
		Error("Renderer must be initialized to render a quad!");
		return;
	}

	UseDefaultShader();
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, s_defaultQuadEBO);
	glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
}

/* Initializes Quad and Shaders for rendering */
auto InitializeRenderer() -> void {
	glEnable(GL_TEXTURE_2D);
	glEnable(GL_DEPTH);

	if (!InitDefaultShader()) {
		Error("Failed to compile default shader!");
		return;
	}

	uint vao = 0;
	glGenVertexArrays(1, &vao);
	glBindVertexArray(vao);

	uint vbo = 0;
	glGenBuffers(1, &vbo);
	glBindBuffer(GL_ARRAY_BUFFER, vbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(s_quadVertexData),
		&s_quadVertexData, GL_STATIC_DRAW);

	uint ebo = 0;
	glGenBuffers(1, &ebo);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(s_quadIndices),
		&s_quadIndices, GL_STATIC_DRAW);

	glVertexAttribPointer(0, 2, GL_FLOAT, false, 16, 0);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(1, 2, GL_FLOAT, false, 16, (void*)8);
	glEnableVertexAttribArray(1);

	s_defaultQuadEBO = ebo;
	s_rendererInitialized = true;
}

/* Sets the OpenGL Viewport */
auto SetViewport(int x, int y, int w, int h) -> void {
	glViewport(x, y, w, h);
}