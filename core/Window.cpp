#include "SzarkCore.h"

static double s_deltaTime = 0;
static bool s_initialized = false;

static void(*s_windowCallback)(GLFWwindow*, WindowEvent);
static void(*s_keyCallback)(int key, int action);
static void(*s_mouseCallback)(int button, int action);
static void(*s_scrollCallback)(double dx, double dy);
static void(*s_cursorCallback)(double x, double y);

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
		// Takes 10 seconds to init because of Windows 10
		// Not really sure how to work around this issue.
		if (glfwInit() == GLFW_FALSE) {
			Error("Failed to initialize GLFW!");
			return nullptr;
		}

		s_initialized = true;
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
		Error("Failed to create GLFW window!");
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
	glfwSwapInterval(1);

	// Make sure GLAD is loaded to get OpenGL functions
	if (!gladLoadGL()) {
		Error("Failed to initialize GLAD!");
		return;
	}

	if (s_windowCallback)
		s_windowCallback(window, WindowEvent::Opened);

	// Keep window open until Close() is called
	double lastTime = glfwGetTime();
	while (!glfwWindowShouldClose(window)) {
		glfwPollEvents();

		double currentTime = glfwGetTime();
		s_deltaTime = (currentTime - lastTime);
		lastTime = currentTime;

		if (s_windowCallback)
			s_windowCallback(window, WindowEvent::Render);

		glfwSwapBuffers(window);
	}

	if (s_windowCallback)
		s_windowCallback(window, WindowEvent::Closed);
}

/* Notifies a window to close */
auto Close(GLFWwindow* window) -> void {
	if (!window) return;
	glfwSetWindowShouldClose(window, true);
}

/* Returns time taken between each frame */
auto GetDeltaTime() -> double { return s_deltaTime; }

/* Changes glfw swap interval */
auto SetVSync(bool enabled) -> void {
	glfwSwapInterval(enabled ? 1 : 0);
}

/* Gets the primary monitor's workspace */
auto GetPrimaryMonitorRect() -> Rect
{
	Rect rect = { 0 };
	auto monitor = glfwGetPrimaryMonitor();

	if (!monitor) {
		Error("Failed to find primary monitor!");
		return rect;
	}

	glfwGetMonitorWorkarea(monitor, &rect.x, &rect.y,
		&rect.width, &rect.height);
	return rect;
}