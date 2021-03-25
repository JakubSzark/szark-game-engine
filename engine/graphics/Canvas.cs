using System.Runtime.CompilerServices;
using static System.Math;
using Szark.Math;

namespace Szark.Graphics
{
    /// <summary>
    /// Provides simple drawing function for Textures.
    /// </summary>
    public class Canvas
    {
        public Texture Target { get; internal set; }
        public Canvas(Texture target) => Target = target;

        /// <summary>
        /// Draws a color at the given x and y.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(int x, int y, Color color) =>
            Target[x, y] = color;

        /// <summary>
        /// Puts a color at the given point
        /// </summary>
        public void Draw(Vector point, Color color) =>
            Draw((int)point.X, (int)point.Y, color);

        /// <summary>
        /// Fills the canvas with a specified color
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(Color color) => Target.Clear(color);

        /// <summary>
        /// Draws a straight line
        /// </summary>
        public void DrawLine(int x1, int y1, int x2, int y2, Color color, int thickness = 1)
        {
            float x, y, step;
            float dx = x2 - x1;
            float dy = y2 - y1;

            float absDX = Abs(dx);
            float absDY = Abs(dy);

            step = absDX >= absDY ? absDX : absDY;

            dx /= step;
            dy /= step;

            x = x1;
            y = y1;

            for (int i = 1; i <= step; i++)
            {
                Draw((int)x, (int)y, color);

                if (thickness > 1)
                {
                    for (int j = 1; j < thickness; j++)
                    {
                        Draw((int)x + j, (int)y, color);
                        Draw((int)x, (int)y + j, color);
                    }
                }

                x += dx;
                y += dy;
            }
        }

        /// <summary>
        /// Draws a line given two points
        /// </summary>
        public void DrawLine(Vector pointA, Vector pointB, Color color, int thickness = 1) =>
            DrawLine((int)pointA.X, (int)pointA.Y, (int)pointB.X, (int)pointB.Y, color, thickness);

        /// <summary>
        /// Draws a hollow rectangle
        /// </summary>
        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            if (width < 0)
            {
                width *= -1;
                x -= width;
            }

            DrawLine(x, y, x + width, y, color);
            DrawLine(x + width - 1, y, x + width - 1, y + height, color);
            DrawLine(x, y + height - 1, x + width, y + height - 1, color);
            DrawLine(x, y, x, y + height, color);
        }

        /// <summary>
        /// Given a point, draws a hollow rectangle
        /// </summary>
        public void DrawRectangle(Vector point, int width, int height, Color color) =>
            DrawRectangle((int)point.X, (int)point.Y, width, height, color);

        /// <summary>
        /// Draws a filled in rectangle
        /// </summary>
        public void FillRectangle(int x, int y, int width, int height, Color color)
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    Draw(x + i, y + j, color);
        }

        /// <summary>
        /// Draws a filled in rectangle with a given point
        /// </summary>
        public void FillRectangle(Vector point, int width, int height, Color color) =>
            FillRectangle((int)point.X, (int)point.Y, width, height, color);

        /// <summary>
        /// Draws a hollow circle
        /// </summary>
        public void DrawCircle(int x0, int y0, int r, Color color)
        {
            x0 += r - 1;
            y0 += r - 1;

            int x = r - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (r << 1);

            while (x >= y)
            {
                Draw(x0 + x, y0 + y, color);
                Draw(x0 + y, y0 + x, color);
                Draw(x0 - y, y0 + x, color);
                Draw(x0 - x, y0 + y, color);
                Draw(x0 - x, y0 - y, color);
                Draw(x0 - y, y0 - x, color);
                Draw(x0 + y, y0 - x, color);
                Draw(x0 + x, y0 - y, color);

                if (err <= 0)
                {
                    y++;
                    err += dy;
                    dy += 2;
                }

                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += dx - (r << 1);
                }
            }
        }

        /// <summary>
        /// Draws a hollow circle given a point
        /// </summary>
        public void DrawCircle(Vector point, int radius, Color color) =>
            DrawCircle((int)point.X, (int)point.Y, radius, color);

        /// <summary>
        /// Draws a filled in circle
        /// </summary>
        public void FillCircle(int x, int y, int radius, Color color)
        {
            for (int i = 0; i < radius * 2; i++)
            {
                for (int j = 0; j < radius * 2; j++)
                {
                    var dist = Sqrt((radius - i) * (radius - i) +
                        (radius - j) * (radius - j));
                    if (dist < radius) Draw(x - 1 + i, y - 1 + j, color);
                }
            }
        }

        /// <summary>
        /// Draws a filled in circle, given a point
        /// </summary>
        public void FillCircle(Vector point, int radius, Color color) =>
            FillCircle((int)point.X, (int)point.Y, radius, color);

        /// <summary>
        /// Draws a hollow triangle
        /// </summary>
        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x2, y2, x3, y3, color);
            DrawLine(x1, y1, x3, y3, color);
        }

        /// <summary>
        /// Draws a hollow triangle, given three points
        /// </summary>
        public void DrawTriangle(Vector pointA, Vector pointB, Vector pointC, Color color) =>
            DrawTriangle((int)pointA.X, (int)pointA.Y, (int)pointB.X, (int)pointB.Y,
                (int)pointC.X, (int)pointC.Y, color);

        /// <summary>
        /// Draws a filled in triangle
        /// </summary>
        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            static float Area(int x1, int y1, int x2, int y2, int x3, int y3) =>
                Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0f);

            var minX = Min(Min(x1, x2), x3);
            var maxX = Max(Max(x1, x2), x3);

            var minY = Min(Min(y1, y2), y3);
            var maxY = Max(Max(y1, y2), y3);

            float a = Area(x1, y1, x2, y2, x3, y3);

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    float a1 = Area(x, y, x2, y2, x3, y3);
                    float a2 = Area(x1, y1, x, y, x3, y3);
                    float a3 = Area(x1, y1, x2, y2, x, y);

                    if (a == a1 + a2 + a3)
                        Draw(x, y, color);
                }
            }
        }

        /// <summary>
        /// Draws a filled in triangle, given three points
        /// </summary>
        public void FillTriangle(Vector pointA, Vector pointB, Vector pointC, Color color) =>
            FillTriangle((int)pointA.X, (int)pointA.Y, (int)pointB.X, (int)pointB.Y,
                (int)pointC.X, (int)pointC.Y, color);

        /// <summary>
        /// Draws a texture on the canvas
        /// </summary>
        public void DrawTexture(int x, int y, Texture texture, int scale = 1)
        {
            for (int i = 0; i < texture.Width; i++)
            {
                for (int j = 0; j < texture.Height; j++)
                {
                    FillRectangle((x + i) * scale, (y + j) * scale,
                        scale, scale, texture[i, j]);
                }
            }
        }

        /// <summary>
        /// Draws a texture on top of the target, give a point
        /// </summary>
        public void DrawTexture(Vector point, Texture texture, int scale = 1) =>
            DrawTexture((int)point.X, (int)point.Y, texture, scale);
    }
}