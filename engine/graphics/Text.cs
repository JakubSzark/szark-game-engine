using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Szark.Graphics;

namespace Szark.Graphics
{
    public static class Text
    {
        private const int FontWidth = 96;
        private const int CellSize = 5;

        //private static byte[] fontData = new byte[FontWidth * FontHeight];

        private static readonly byte[] rawFontData =
        {
            0,32,80,80,32,136,24,32,32,32,0,0,0,0,0,8,112,96,112,112,48,
            112,112,112,112,112,0,0,16,0,64,96,112,248,240,248,240,248,248,248,136,
            248,248,136,128,216,248,248,248,248,248,248,248,136,136,136,136,136,248,112,128,
            112,32,64,248,240,248,240,248,248,248,136,248,248,136,128,216,248,248,248,248,
            248,248,248,136,136,136,136,136,248,64,48,32,96,0,0,0,32,80,248,112,16,96,32,64,
            16,80,32,48,0,0,16,80,32,16,16,80,
            64,64,16,80,80,32,32,32,112,32,16,152,136,136,128,136,128,128,128,136,
            32,32,144,128,168,136,136,136,136,136,128,32,136,136,136,80,80,16,64,64,
            16,80,32,136,136,128,136,128,128,128,136,32,32,144,128,168,136,136,136,136,
            136,128,32,136,136,136,80,80,16,32,32,32,32,96,0,0,32,0,80,64,32,168,0,64,16,
            32,112,32,112,0,32,80,32,32,32,112,
            112,112,16,112,112,0,0,64,0,16,48,168,248,248,128,136,248,248,184,248,
            32,32,224,128,136,136,136,248,136,248,248,32,136,136,136,32,32,32,64,32,
            16,136,16,248,248,128,136,248,248,184,248,32,32,224,128,136,136,136,248,136,
            248,248,32,136,136,136,32,32,32,16,64,32,16,80,0,0,0,0,248,112,64,144,0,64,
            16,80,32,64,0,0,64,80,32,64,16,16,
            16,80,16,80,16,32,32,32,112,32,0,152,136,136,128,136,128,128,136,136,
            32,32,144,128,136,136,136,128,248,160,8,32,136,80,168,80,32,64,64,16,
            16,0,0,136,136,128,136,128,128,136,136,32,32,144,128,136,136,136,128,248,
            160,8,32,136,80,168,80,32,64,0,32,32,32,0,0,0,32,0,80,32,136,104,0,32,32,0,
            0,0,0,32,128,112,112,112,112,16,
            112,112,16,112,16,0,64,16,0,64,32,64,136,240,248,240,248,128,248,136,
            248,224,136,248,136,136,248,128,16,144,248,32,248,32,80,136,32,248,112,8,
            112,0,0,136,240,248,240,248,128,248,136,248,224,136,248,136,136,248,128,16,
            144,248,32,248,32,80,136,32,248,0,48,32,96,0,0
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void DrawCellRow(Canvas gfx, int rowX, int rowY, int xPos, int yPos, Color color)
        {
            byte row = rawFontData[rowY * FontWidth + rowX];
            int bit = 0x80;

            for (int i = 0; i < CellSize; i++)
            {
                if ((row & bit) > 0)
                    gfx.Draw(xPos + i, yPos, color);
                bit >>= 1;
            }
        }

        /// <summary>
        /// Draws a character on the screen
        /// </summary>
        public static void DrawChar(this Canvas gfx, int x, int y, char ch, Color color)
        {
            byte b = (byte)ch;
            if (b < 32 || b >= 127) return;

            if (b >= 97 && b <= 122) b -= 32;

            var rowX = b - 32;
            for (int rowY = 0; rowY < CellSize; rowY++)
                DrawCellRow(gfx, rowX, rowY, x, y + rowY, color);
        }

        /// <summary>
        /// Draws a string onto the screen
        /// </summary>
        public static void DrawString(this Canvas gfx, int x, int y,
            string text, Color color, int letterSpacing = 1)
        {
            int i = 0;
            foreach (var ch in text)
            {
                DrawChar(gfx, x + i, y, ch, color);
                i += CellSize + letterSpacing;
            }
        }
    }
}
