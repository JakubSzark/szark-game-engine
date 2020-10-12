using System;
using System.IO;

namespace Szark.Graphics
{
    public class Texture
    {
        public Color[] Pixels { get; private set; }

        public uint Width { get; private set; }
        public uint Height { get; private set; }

        public Color this[int x, int y]
        {
            get => Read(x, y);
            set => Write(x, y, value);
        }

        /// <summary>
        /// Creates an empty texture
        /// </summary>
        public Texture(uint width, uint height)
        {
            Width = width;
            Height = height;
            Pixels = new Color[width * height];
        }

        /// <summary>
        /// Creates a texture from a bitmap file
        /// </summary>
        public Texture(string filePath)
        {
            try
            {
                using var stream = File.Open(filePath, FileMode.Open);
                using var reader = new BinaryReader(stream);

                ushort identity = reader.ReadUInt16();
                uint size = reader.ReadUInt32();
                uint reserved = reader.ReadUInt32();
                uint offset = reader.ReadUInt32();

                uint dibSize = reader.ReadUInt32();
                uint width = reader.ReadUInt32();
                uint height = reader.ReadUInt32();

                ushort planes = reader.ReadUInt16();
                ushort bpp = reader.ReadUInt16();
                uint compression = reader.ReadUInt32();
                uint totalSize = reader.ReadUInt32();

                stream.Seek(offset, SeekOrigin.Begin);
                
                Pixels = new Color[width * height];
                Width = width;
                Height = height;

                int x = 0;
                int y = (int)height - 1;

                int pixelSize = bpp / 8;
                Span<byte> pixel = stackalloc byte[pixelSize];
                while (reader.Read(pixel) > 0)
                {
                    this[x, y] = new Color()
                    {
                        r = pixel[2],
                        g = pixel[1],
                        b = pixel[0]
                    };

                    x++;
                    if (x >= width)
                    {
                        y--;
                        x = 0;
                    }
                }
            }
            catch (FileNotFoundException) {
                throw new FileNotFoundException("Could not find bitmap file!");
            }
            catch (Exception) {
                throw new Exception("Invalid bitmap file!");
            }
        }

        public Color Read(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return new Color();
            return Pixels[y * Width + x];
        }

        public void Write(int x, int y, Color color)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                Pixels[y * Width + x] = color;
        }

        public void Clear(Color color)
        {
            for (int i = 0; i < Pixels.Length; i++)
                Pixels[i] = color;
        }

        public Canvas GetCanvas() => new Canvas(this);

        public uint GenerateID() =>
            Core.GenerateTextureID(Pixels, Width, Height);

        public uint Update(uint textureID) =>
            Core.UpdateTexture(textureID, Pixels, Width, Height);
    }
}
