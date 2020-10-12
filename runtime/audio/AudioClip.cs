using System.IO;
using System;

namespace Szark.Audio
{
    public class AudioClip
    {
        private readonly uint source;

        public AudioClip(string filePath)
        {
            try
            {
                using var stream = File.Open(filePath, FileMode.Open);
                using var reader = new BinaryReader(stream);

                string signature = new string(reader.ReadChars(4));
                
                if (signature != "RIFF") 
                    throw new NotSupportedException("Invalid signature!");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));

                if (format != "WAVE") 
                    throw new NotSupportedException("Invalid format!");

                string format_signature = new string(reader.ReadChars(4));

                if (format_signature != "fmt ") 
                    throw new NotSupportedException("Invalid format signature!");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                var channels = reader.ReadInt16();
                var sampleRate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                var bitsPerSample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));

                if (data_signature != "data") 
                    throw new NotSupportedException("Invalid data signature");

                int data_chunk_size = reader.ReadInt32();

                var audioData = reader.ReadBytes((int)reader.BaseStream.Length);
                int soundFormat = 0;

                switch (channels)
                {
                    case 1:
                        soundFormat = bitsPerSample == 8 ? 0x1100 : 0x1101;
                        break;
                    case 2:
                        soundFormat = bitsPerSample == 8 ? 0x1102 : 0x1103;
                        break;

                    default: throw new NotSupportedException();
                }

                source = Core.GenerateAudioClipID(soundFormat, audioData, 
                    (uint)audioData.Length, (uint)sampleRate);

                return;
            }
            catch (FileNotFoundException) {
                throw new FileNotFoundException("WAV file could not be found!");
            }
            catch (Exception) {
                throw new NotSupportedException("Invalid WAV file!");
            }
        }

        public void Play(int volume = 1, bool loop = false) =>
            Core.PlayAudioClip(source, volume, loop);

        public void Stop() => Core.StopAudioClip(source);
    }
}