using System;
using System.Globalization;
using Szark.Math;

namespace Szark.Graphics
{
    public struct Color
    {
        public byte R, G, B;

        public Color(byte red, byte green, byte blue) =>
            (R, G, B) = (red, green, blue);

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
                R = (byte)Mathf.Lerp(first.R, second.R, time),
                G = (byte)Mathf.Lerp(first.G, second.G, time),
                B = (byte)Mathf.Lerp(first.B, second.B, time),
            };
        }

        /// <summary>
        /// Creates a color based on the HSV format and converted in RGBA.
        /// [Hue(0-360), Saturation(0-100), Value(0-100), Alpha(0-255)]
        /// </summary>
        public static Color FromHSV(float H, float S, float V)
        {
            float NR, NG, NB;
            float hf = (H %= 360) / 60.0f;
            int i = (int)System.Math.Floor(hf);

            float pv = V * (1 - S);
            float qv = V * (1 - S * (hf - i));
            float tv = V * (1 - S * (1 - (hf - i)));

            switch (i)
            {
                case 0: NR = V; NG = tv; NB = pv; break;
                case 1: NR = qv; NG = V; NB = pv; break;
                case 2: NR = pv; NG = V; NB = tv; break;
                case 3: NR = pv; NG = qv; NB = V; break;
                case 4: NR = tv; NG = pv; NB = V; break;
                default: NR = V; NG = pv; NB = qv; break;
            }

            return new Color
            {
                R = (byte)(NR * 255f),
                G = (byte)(NG * 255f),
                B = (byte)(NB * 255f),
            };
        }

        /// <summary>
        /// Creates a color based on the HSL format and converted to RGBA.
        /// [Hue(0-360), Saturation(0-100), Lightness(0-100), Alpha(0-255)]
        /// </summary>
        public static Color FromHSL(float h, float s, float l)
        {
            h = Mathf.Clamp(h / 360, 0, 1);
            s = Mathf.Clamp(s / 100, 0, 1);
            l = Mathf.Clamp(l / 100, 0, 1);

            static float GetHue(float p1, float q1, float t)
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
                R = (byte)System.Math.Round(GetHue(p, q, h + 0.333f)),
                G = (byte)System.Math.Round(GetHue(p, q, h)),
                B = (byte)System.Math.Round(GetHue(p, q, h - 0.333f)),
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
                R = (byte)int.Parse(hex.Substring(1, 2), (NumberStyles)512),
                G = (byte)int.Parse(hex.Substring(3, 2), (NumberStyles)512),
                B = (byte)int.Parse(hex.Substring(5, 2), (NumberStyles)512)
            };
        }

        // -- Object Overrides --

        public override bool Equals(object? obj) =>
            obj is Color color && R == color.R &&
                G == color.G && B == color.B;

        public override int GetHashCode() =>
            HashCode.Combine(R, G, B);

        public override string ToString() => $"({R},{G},{B})";

        // -- Arithmetic Operators --

        public static Color operator +(Color f, Color p) =>
            new Color(
                    (byte)Mathf.Clamp01(f.R + p.R),
                    (byte)Mathf.Clamp01(f.G + p.G),
                    (byte)Mathf.Clamp01(f.B + p.B)
                );

        public static Color operator -(Color f, Color p) =>
            new Color(
                    (byte)Mathf.Clamp01(f.R - p.R),
                    (byte)Mathf.Clamp01(f.G - p.G),
                    (byte)Mathf.Clamp01(f.B - p.B)
                );

        public static Color operator *(Color f, float t) =>
            new Color(
                    (byte)Mathf.Clamp01(f.R * t),
                    (byte)Mathf.Clamp01(f.G * t),
                    (byte)Mathf.Clamp01(f.B * t)
                );

        // -- Equality Operators --

        public static bool operator ==(Color a, Color b) =>
            a.R == b.R && a.G == b.G && a.B == b.B;

        public static bool operator !=(Color a, Color b) =>
            a.R != b.R || a.G != b.G || a.B != b.B;
    }
}