#include "SzarkCore.h"

static void(*s_errorCallback)(const char*);
static void(*s_windowCallback)(GLFWwindow*, WindowEvent);
static void(*s_keyCallback)(int key, int action);
static void(*s_mouseCallback)(int button, int action);
static void(*s_scrollCallback)(double dx, double dy);
static void(*s_cursorCallback)(double x, double y);

static bool s_initialized = false;
static bool s_rendererInitialized = false;
static bool s_gladLoaded = false;

static double s_deltaTime = 0;

static uint s_defaultProgramID;
static uint s_defaultQuadEBO;

static const char* s_defaultVertexShader =
"#version 420\n"
"layout(location = 0) in vec2 pos;"
"layout(location = 1) in vec2 tex;"
"out vec2 texCoord;"
"void main() {"
"	texCoord = tex;"
"	gl_Position = vec4(pos.x, -pos.y, 0, 1.0);"
"}";

static const char* s_defaultFragmentShader =
"#version 420\n"
"out vec4 FragColor;"
"in vec2 texCoord;"
"uniform sampler2D tex;"
"void main() {"
"	FragColor = texture(tex, texCoord);"
"}";

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

/* GLFW keyboard input callback */
static void keyCallback(GLFWwindow* window, int key, int scancode, int action, int mods) {
	if (s_keyCallback) s_keyCallback(key, action);
}

/* GLFW cursor position callback */
static void cursorPosCallback(GLFWwindow* window, double xpos, double ypos) {
	if (s_cursorCallback) s_cursorCallback(xpos, ypos);
}

/* GLFW mouse button input callback */
static void mouseCallback(GLFWwindow* window, int button, int action, int mods) {
	if (s_mouseCallback) s_mouseCallback(button, action);
}

/* GLFW mouse scroll input callback */
static void scrollCallback(GLFWwindow* window, double xoffset, double yoffset) {
	if (s_scrollCallback) s_scrollCallback(xoffset, yoffset);
}

/* Prints a most recent shader error log and deletes the shader */
static void shaderCompileError(uint id) {
	int length = 0;
	glGetShaderiv(id, GL_INFO_LOG_LENGTH, &length);

	std::vector<char> logBuffer(length);
	glGetShaderInfoLog(id, length, nullptr, logBuffer.data());
	std::string errorLog(logBuffer.begin(), logBuffer.end());

	if (s_errorCallback) s_errorCallback(errorLog.c_str());
	glDeleteShader(id);
}

/* Prints a most recent program link error log and deletes the program */
static void linkCompileError(uint id) {
	int length = 0;
	glGetProgramiv(id, GL_INFO_LOG_LENGTH, &length);

	std::vector<char> logBuffer(length);
	glGetProgramInfoLog(id, length, nullptr, logBuffer.data());
	std::string errorLog(logBuffer.begin(), logBuffer.end());

	if (s_errorCallback) s_errorCallback(errorLog.c_str());
	glDeleteProgram(id);
}

/* Initializes Szark Core libraries */
auto InitializeLibraries() -> bool {
	// Takes 10 seconds to init because of Windows 10
	// Not really sure how to work around this issue.
	if (glfwInit() == GLFW_FALSE) {
		if (s_errorCallback)
			s_errorCallback("Failed to initialize GLFW!");
		return false;
	}

	s_initialized = true;
	return true;
}

/* Sets a callback for any errors that occur */
auto SetErrorCallback(void(*callback)(const char* m)) -> void {
	if (callback) s_errorCallback = callback;
}

/* Sets a callback for any window events */
auto SetWindowEventCallback(void(*callback)(GLFWwindow* w, WindowEvent e)) -> void {
	if (callback) s_windowCallback = callback;
}

/* Sets a callback for mouse wheel scroll input */
auto SetScrollCallback(void(*callback)(double dx, double dy)) {
	if (callback) s_scrollCallback = callback;
}

/* Sets a callback to listen for mouse button events */
auto SetMouseCallback(void(*callback)(int button, int action)) {
	if (callback) s_mouseCallback = callback;
}

/* Sets a callback to listen for key events */
auto SetKeyCallback(void(*callback)(int key, int action)) {
	if (callback) s_keyCallback = callback;
}

/* Sets the callback for the cursor */
auto SetCursorCallback(void(*callback)(double x, double y)) {
	if (callback) s_cursorCallback = callback;
}

/* Creates a GLFW window with a desired configuration */
auto Create(
	const char* title, uint width, uint height, bool fullscreen
) -> GLFWwindow* {
	// You must initialize libraries first
	if (!s_initialized) {
		if (s_errorCallback)
			s_errorCallback("You must initialize libraries first!");
		return nullptr;
	}

	// Make sure values have a default if incorrect
	if (width == 0) width = 800;
	if (title == nullptr) title = "New Window";
	if (height == 0) height = 600;

	if (fullscreen) {
		auto rect = GetPrimaryMonitorRect();
		width = (uint)rect.width;
		height = (uint)rect.height;
	}

	// We create the GLFW window here
	glfwWindowHint(GLFW_RESIZABLE, false);
	glfwWindowHint(GLFW_DECORATED, !fullscreen);
	

	GLFWwindow* window = glfwCreateWindow(
		width, height, title, nullptr, nullptr
	);

	if (!window) {
		if (s_errorCallback)
			s_errorCallback("Failed to create GLFW window!");
		return nullptr;
	}

	return window;
}

/* Shows a GLFW window on screen */
auto Show(GLFWwindow* window) -> void {
	if (!window) return;

	glfwSetKeyCallback(window, keyCallback);
	glfwSetCursorPosCallback(window, cursorPosCallback);
	glfwSetMouseButtonCallback(window, mouseCallback);
	glfwSetScrollCallback(window, scrollCallback);

	glfwMakeContextCurrent(window);
	
	// Make sure GLAD is loaded to get OpenGL functions
	if (!gladLoadGL()) {
		if (s_errorCallback)
			s_errorCallback("Failed to initialize GLAD!");
		return;
	}

	s_gladLoaded = true;
	if (s_windowCallback)
		s_windowCallback(window, WindowEvent::Opened);

	// Keep window open until Close() is called
	double lastTime = glfwGetTime();
	while (!glfwWindowShouldClose(window)) {
		double currentTime = glfwGetTime();
		s_deltaTime = (currentTime - lastTime);
		lastTime = currentTime;
		glfwPollEvents();
		glfwSwapBuffers(window);
		if (s_windowCallback)
			s_windowCallback(window, WindowEvent::Render);
	}

	if (s_windowCallback)
		s_windowCallback(window, WindowEvent::Closed);
}

/* Notifies a window to close */
auto Close(GLFWwindow* window) -> void {
	if (!window) return;
	glfwSetWindowShouldClose(window, true);
}

/* Terminates any libraries in use */
auto TerminateLibraries() -> void {
	if (!s_initialized) return;
	glfwTerminate();
	s_initialized = false;
}

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

/* Uses the Shader program */
auto inline UseShader(uint id) -> void {
	glUseProgram(id);
}

/* Uses the default renderer shader */
auto inline UseDefaultShader() -> void {
	UseShader(s_defaultProgramID);
}

/* Uses a texture */
auto inline UseTexture(uint id) -> void {
	glBindTexture(GL_TEXTURE_2D, id);
}

/* Renders a quad on screen */
auto RenderQuad() -> void {
	// Renderer must be initialized to render a quad
	if (!s_rendererInitialized) {
		if (s_errorCallback)
			s_errorCallback("Renderer must be initialized to render a quad!");
		return;
	}

	UseDefaultShader();
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, s_defaultQuadEBO);
	glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
}

auto GetDeltaTime() -> double {
	return s_deltaTime;
}

/* Compiles a Shader program from a vertex and fragment shader */
auto CompileShader(const char* vertexSrc, const char* fragmentSrc) -> uint {
	if (!s_gladLoaded) {
		if (s_errorCallback)
			s_errorCallback("Cannot compile shader without GLAD loaded!");
		return 0;
	}

	int success = 0;
	uint vertID = glCreateShader(GL_VERTEX_SHADER);
	uint fragID = glCreateShader(GL_FRAGMENT_SHADER);

	glShaderSource(vertID, 1, &vertexSrc, nullptr);
	glCompileShader(vertID);

	glGetShaderiv(vertID, GL_COMPILE_STATUS, &success);
	
	if (success == GL_FALSE) {
		shaderCompileError(vertID);
		return 0;
	}

	glShaderSource(fragID, 1, &fragmentSrc, nullptr);
	glCompileShader(fragID);

	glGetShaderiv(fragID, GL_COMPILE_STATUS, &success);
	
	if (success == GL_FALSE) { 
		shaderCompileError(fragID); 
		return 0;
	}

	int programID = glCreateProgram();
	glAttachShader(programID, vertID);
	glAttachShader(programID, fragID);
	glLinkProgram(programID);

	glGetProgramiv(programID, GL_LINK_STATUS, &success);

	if (success == GL_FALSE) {
		linkCompileError(programID);
		glDeleteShader(vertID);
		glDeleteShader(fragID);
		return 0;
	}

	glDetachShader(programID, vertID);
	glDetachShader(programID, fragID);

	return programID;
}

/* Initializes Quad and Shaders for rendering */
auto InitializeRenderer() -> void {
	// Glad needs to be loaded to call OpenGL functions
	if (!s_gladLoaded) {
		if (s_errorCallback)
			s_errorCallback("Cannot initialize renderer without GLAD loaded!");
		return;
	}

	glEnable(GL_TEXTURE_2D);
	glEnable(GL_DEPTH);

	s_defaultProgramID = CompileShader(s_defaultVertexShader, 
		s_defaultFragmentShader);
	
	if (s_defaultProgramID == 0) {
		if (s_errorCallback)
			s_errorCallback("Failed to compile default shader!");
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

auto GetPrimaryMonitorRect() -> Rect
{
	Rect rect = { 0 };
	auto monitor = glfwGetPrimaryMonitor();
	glfwGetMonitorWorkarea(monitor, &rect.x, &rect.y, 
		&rect.width, &rect.height);
	return rect;
}

auto SetViewport(int x, int y, int w, int h) -> void {
	glViewport(x, y, w, h);
}

/* Initializes OpenAL */
auto InitializeAudioContext() -> void {
	auto device = alcOpenDevice(nullptr);
	
	if (device == nullptr)
	{
		if (s_errorCallback)
			s_errorCallback("Failed to find default audio device!");
		return;
	}

	auto context = alcCreateContext(device, nullptr);
	
	if (!alcMakeContextCurrent(context))
	{
		if (s_errorCallback)
			s_errorCallback("OpenAL is not installed on this device!");
		return;
	}
}

/* Plays an audio clip from a generated id */
auto PlayAudioClip(uint id, int volume, bool loop) -> void {
	alSourcei(id, AL_LOOPING, loop);
	alSourcei(id, AL_GAIN, volume);
	alSourcePlay(id);
}

/* Stop audio clip from playing */
auto StopAudioClip(uint id) -> void {
	alSourceStop(id);
}

/* Generates an id for an audio clip */
auto GenerateAudioClipID(int format, const char* buffer, uint length, uint freq) -> uint
{
	uint source = 0;
	alGenSources(1, &source);

	uint id = 0;
	alGenBuffers(1, &id);
	alBufferData(id, format, buffer, length, freq);
	alSourcei(source, AL_BUFFER, id);
	return source;
}

/* Empty main to compile library */
int main() { }