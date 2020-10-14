#include "SzarkCore.h"

/* Initializes OpenAL */
auto InitializeAudioContext() -> void {
	auto device = alcOpenDevice(nullptr);

	if (!device) {
		Error("Failed to find default audio device!");
		return;
	}

	auto context = alcCreateContext(device, nullptr);

	if (!alcMakeContextCurrent(context)) {
		Error("OpenAL is not installed on this device!");
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
auto CreateAudioClip(int format, const char* buffer, uint length, uint freq) -> AudioClip
{
	uint source = 0;
	alGenSources(1, &source);

	uint id = 0;
	alGenBuffers(1, &id);
	alBufferData(id, format, buffer, length, freq);
	alSourcei(source, AL_BUFFER, id);
	return { source, id };
}

/* Destroys an OpenGL audio clip */
auto DestroyAudioClip(AudioClip clip) -> void {
	alDeleteBuffers(1, &clip.buffer);
	alDeleteSources(1, &clip.source);
}