#pragma once

#define EXPORT __declspec( dllexport )

#include <string>
#include <vector>

#include "vendor/glad/glad/glad.h"
#include "vendor/glfw/glfw3.h"

#include "vendor/al/al.h"
#include "vendor/al/alc.h"

using uint = uint32_t;

enum class WindowEvent {
	Opened,
	Closed,
	Render,
};

struct Point { double x, y; };
struct Rect { int x, y, width, height; };
struct InputValue { int control, action; };
struct Color { unsigned char r, g, b; };
struct AudioClip { uint source, buffer; };

auto Error(const char* msg) -> void;
auto InitDefaultShader() -> bool;

extern "C"
{
	EXPORT auto SetErrorCallback(void(*c)(const char* m)) -> void;
	EXPORT auto SetWindowEventCallback(void(*c)(GLFWwindow* w, WindowEvent e)) -> void;

	EXPORT auto SetScrollCallback(void(*c)(double dx, double dy));
	EXPORT auto SetMouseCallback(void(*c)(int button, int action));
	EXPORT auto SetKeyCallback(void(*c)(int key, int action));
	EXPORT auto SetCursorCallback(void(*c)(double x, double y));

	EXPORT auto Show(GLFWwindow* window) -> void;
	EXPORT auto Create(const char*, uint, uint, bool)->GLFWwindow*;
	EXPORT auto Close(GLFWwindow* window) -> void;

	EXPORT auto GenerateTextureID(Color*, uint, uint)->uint;
	EXPORT auto UpdateTexture(uint, Color*, uint, uint) -> void;

	EXPORT auto UseShader(uint) -> void;
	EXPORT auto UseDefaultShader() -> void;
	EXPORT auto UseTexture(uint) -> void;
	EXPORT auto RenderQuad() -> void;

	EXPORT auto GetDeltaTime() -> double;
	EXPORT auto CompileShader(const char*, const char*)->uint;
	EXPORT auto InitializeRenderer() -> void;

	EXPORT auto GetPrimaryMonitorRect()->Rect;
	EXPORT auto SetViewport(int, int, int, int) -> void;

	EXPORT auto InitializeAudioContext() -> void;
	EXPORT auto PlayAudioClip(uint, int, bool) -> void;
	EXPORT auto StopAudioClip(uint id) -> void;
	EXPORT auto CreateAudioClip(int format, const char* buffer,
		uint length, uint freq)->AudioClip;
	EXPORT auto DestroyAudioClip(AudioClip clip) -> void;

	EXPORT auto SetVSync(bool enabled) -> void;
}