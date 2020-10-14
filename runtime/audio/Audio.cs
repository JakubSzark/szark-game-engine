using System.IO;
using System.Runtime.InteropServices;
using System;

namespace Szark.Audio
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioClip : IDisposable
    {
        internal uint source, buffer;

        public void Dispose() =>
            Core.DestroyAudioClip(this);

        /// <summary>
        /// Plays the Audioclip
        /// </summary>
        public void Play(int volume = 1, bool loop = false) =>
            Core.PlayAudioClip(source, volume, loop);

        /// <summary>
        /// Stops the Audioclip from playing
        /// </summary>
        public void Stop() => Core.StopAudioClip(source);
    }

    /// <summary>
    /// A clip of audio that can be played.
    /// </summary>
    public static class Audio
    {
        public static AudioClip? ReadFile(string filePath)
        {
            try
            {
                using var stream = File.Open(filePath, FileMode.Open);
                using var reader = new BinaryReader(stream);

                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Invalid signature");

                int riff_chunck_size = reader.ReadInt32();
                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Invalid format");

                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Incorrect format signature");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                var channels = reader.ReadInt16();
                var sampleRate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                var bitsPerSample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Incorrect data");
                int data_chunk_size = reader.ReadInt32();

                var audioData = reader.ReadBytes((int)reader.BaseStream.Length);

                int soundFormat = channels switch
                {
                    1 => bitsPerSample == 8 ? 0x1100 : 0x1101,
                    2 => bitsPerSample == 8 ? 0x1102 : 0x1103,
                    _ => throw new NotSupportedException(),
                };

                return Core.CreateAudioClip(soundFormat, audioData,
                    (uint)audioData.Length, (uint)sampleRate);
            }
            catch (FileNotFoundException)
            {
                Game.Instance?.Error($"WAV file {filePath} could not be found!");
            }
            catch (Exception e)
            {
                Game.Instance?.Error($"{filePath} is an invalid WAV file!" +
                    $"\n{e}");
            }

            return null;
        }
    }
}