#include "SzarkCore.h"

static uint s_defaultProgramID;

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

/* Prints a most recent shader error log and deletes the shader */
static void shaderCompileError(uint id) {
	int length = 0;
	glGetShaderiv(id, GL_INFO_LOG_LENGTH, &length);

	std::vector<char> logBuffer(length);
	glGetShaderInfoLog(id, length, nullptr, logBuffer.data());
	std::string errorLog(logBuffer.begin(), logBuffer.end());

	Error(errorLog.c_str());
	glDeleteShader(id);
}

/* Prints a most recent program link error log and deletes the program */
static void linkCompileError(uint id) {
	int length = 0;
	glGetProgramiv(id, GL_INFO_LOG_LENGTH, &length);

	std::vector<char> logBuffer(length);
	glGetProgramInfoLog(id, length, nullptr, logBuffer.data());
	std::string errorLog(logBuffer.begin(), logBuffer.end());

	Error(errorLog.c_str());
	glDeleteProgram(id);
}

/* Compiles a Shader program from a vertex and fragment shader */
auto CompileShader(const char* vertexSrc, const char* fragmentSrc) -> uint {
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

/* Creates a default flat shaded shader */
auto InitDefaultShader() -> bool {
	s_defaultProgramID = CompileShader(s_defaultVertexShader,
		s_defaultFragmentShader);
	return s_defaultProgramID != 0;
}

/* Uses the default renderer shader */
auto inline UseDefaultShader() -> void {
	UseShader(s_defaultProgramID);
}

/* Uses the Shader program */
auto inline UseShader(uint id) -> void {
	glUseProgram(id);
}