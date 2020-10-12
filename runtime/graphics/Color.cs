using System;
using System.Globalization;
using Szark.Math;

namespace Szark.Graphics
{
    public struct Color
    {
        public byte r, g, b;

        public Color(byte red, byte green, byte blue)
        {
            r = red;
            g = green;
            b = blue;
        }

        // -- Constants --

        public readonly static Color Clear = new Color(0, 0, 0);
        public readonly static Color White = new Color(255, 255, 255);
        public readonly static Color Grey = new Color(192, 192, 192);
        public readonly static Color Black = new Color(0, 0, 0);

        public readonly static Color Red = new Color(255, 0, 0);
        public readonly static Color Green = new Color(0, 255, 0);
        public readonly static Color Blue = new Color(0, 0, 255);

        public readonly static Color Yellow = new Color(255, 255, 0);
        public readonly static Color Magenta = new Color(255, 0, 255);
        public readonly static Color Cyan = new Color(0, 255, 255);

        // -- Static Methods --

        /// <summary>
        /// Linearly interpolates the first color and the
        /// second color based on the time value.
        /// </summary>
        public static Color Lerp(Color first, Color second, float time)
        {
            return new Color
            {
                r = (byte)Mathf.Lerp(first.r, second.r, time),
                g = (byte)Mathf.Lerp(first.g, second.g, time),
                b = (byte)Mathf.Lerp(first.b, second.b, time),
            };
        }

        /// <summary>
        /// Creates a color based on the HSV format and converted in RGBA.
        /// [Hue(0-360), Saturation(0-100), Value(0-100), Alpha(0-255)]
        /// </summary>
        public static Color FromHSV(float H, float S, float V, byte a = 255)
        {
            float R, G, B;
            float hf = (H %= 360) / 60.0f;
            int i = (int)System.Math.Floor(hf);

            float pv = V * (1 - S);
            float qv = V * (1 - S * (hf - i));
            float tv = V * (1 - S * (1 - (hf - i)));

            switch (i)
            {
                case 0: R = V; G = tv; B = pv; break;
                case 1: R = qv; G = V; B = pv; break;
                case 2: R = pv; G = V; B = tv; break;
                case 3: R = pv; G = qv; B = V; break;
                case 4: R = tv; G = pv; B = V; break;
                default: R = V; G = pv; B = qv; break;
            }

            return new Color
            {
                r = (byte)(R * 255f),
                g = (byte)(G * 255f),
                b = (byte)(B * 255f),
            };
        }

        /// <summary>
        /// Creates a color based on the HSL format and converted to RGBA.
        /// [Hue(0-360), Saturation(0-100), Lightness(0-100), Alpha(0-255)]
        /// </summary>
        public static Color FromHSL(float h, float s, float l, byte a = 255)
        {
            h = Mathf.Clamp(h / 360, 0, 1);
            s = Mathf.Clamp(s / 100, 0, 1);
            l = Mathf.Clamp(l / 100, 0, 1);

            float GetHue(float p1, float q1, float t)
            {
                if (t < 0) t += 1;
                if (t > 1) t -= 1;
                if (t < 0.16f) return p1 + (q1 - p1) * 6f * t;
                if (t < 0.5f) return q1;
                if (t < 0.666f) return p1 + (q1 - p1) * (0.666f - t) * 6f;
                return p1;
            }

            float q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            float p = 2 * l - q;

            return new Color
            {
                r = (byte)System.Math.Round(GetHue(p, q, h + 0.333f)),
                g = (byte)System.Math.Round(GetHue(p, q, h)),
                b = (byte)System.Math.Round(GetHue(p, q, h - 0.333f)),
            };
        }

        /// <summary>
        /// Creates a color from a Hex Code.
        /// </summary>
        public static Color FromHex(string hex)
        {
            if (hex[0] != '#' || hex.Length != 7)
                return new Color();

            return new Color
            {
                r = (byte)int.Parse(hex.Substring(1, 2), (NumberStyles)512),
                g = (byte)int.Parse(hex.Substring(3, 2), (NumberStyles)512),
                b = (byte)int.Parse(hex.Substring(5, 2), (NumberStyles)512)
            };
        }

        // -- Object Overrides --

        public override bool Equals(object obj) =>
            obj is Color color && r == color.r &&
                g == color.g && b == color.b;

        public override int GetHashCode() =>
            HashCode.Combine(r, g, b);

        public override string ToString() =>
            $"({r},{g},{b})";

        // -- Arithmetic Operators --

        public static Color operator +(Color f, Color p) =>
            new Color((byte)(f.r + p.r), (byte)(f.g + p.g),
                (byte)(f.b + p.b));

        public static Color operator -(Color f, Color p) =>
            new Color((byte)(f.r - p.r), (byte)(f.g - p.g),
                (byte)(f.b - p.b));

        public static Color operator *(Color f, float t) =>
            new Color((byte)(f.r * t), (byte)(f.g * t),
                (byte)(f.b * t));

        // -- Equality Operators --

        public static bool operator ==(Color a, Color b) =>
            a.r == b.r && a.g == b.g &&
                a.b == b.b;

        public static bool operator !=(Color a, Color b) =>
            a.r != b.r || a.g != b.g ||
                a.b != b.b;
    }
}