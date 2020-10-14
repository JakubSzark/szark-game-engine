#include "SzarkCore.h"

static void(*s_errorCallback)(const char*);

/* Sends an error message to the runtime */
auto Error(const char* msg) -> void {
	if (s_errorCallback) s_errorCallback(msg);
}

/* Sets a callback for any errors that occur */
auto SetErrorCallback(void(*callback)(const char* m)) -> void {
	if (callback) s_errorCallback = callback;
}

/* Empty main to compile library */
int main() { }