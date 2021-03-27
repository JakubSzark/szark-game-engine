using System.Runtime.CompilerServices;

namespace Szark.Math
{
    public static class Mathf
    {
        /// <summary>Mutiply by this to convert to Radians</summary>
        public const float DEG2RAD = (float)(System.Math.PI / 180.0f);

        /// <summary>Mutiply by this to convert to Degrees</summary>
        public const float RAD2DEG = (float)(180.0f / System.Math.PI);

        readonly static byte[] hash =
        {
            208,34,231,213,32,248,233,56,161,78,24,140,71,48,140,254,245,255,247,247,40,
            185,248,251,245,28,124,204,204,76,36,1,107,28,234,163,202,224,245,128,167,204,
            9,92,217,54,239,174,173,102,193,189,190,121,100,108,167,44,43,77,180,204,8,81,
            70,223,11,38,24,254,210,210,177,32,81,195,243,125,8,169,112,32,97,53,195,13,
            203,9,47,104,125,117,114,124,165,203,181,235,193,206,70,180,174,0,167,181,41,
            164,30,116,127,198,245,146,87,224,149,206,57,4,192,210,65,210,129,240,178,105,
            228,108,245,148,140,40,35,195,38,58,65,207,215,253,65,85,208,76,62,3,237,55,89,
            232,50,217,64,244,157,199,121,252,90,17,212,203,149,152,140,187,234,177,73,174,
            193,100,192,143,97,53,145,135,19,103,13,90,135,151,199,91,239,247,33,39,145,
            101,120,99,3,186,86,99,41,237,203,111,79,220,135,158,42,30,154,120,67,87,167,
            135,176,183,191,253,115,184,21,233,58,129,233,142,39,128,211,118,137,139,255,
            114,20,218,113,154,27,127,246,250,1,8,198,250,209,92,222,173,21,88,102,219
        };

        /// <summary>
        /// Interpolates from a -> b based on t
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t) =>
            (1 - t) * a + t * b;

        /// <summary>
        /// Locks the given value between a min and a max
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float val, float min, float max) =>
            val < min ? min : val > max ? max : val;

        /// <summary>
        /// Locks the given value between a 0 and 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float val) =>
            val < 0 ? 0 : val > 1 ? 1 : val;

        /// <summary>
        /// Smoothly curves given x between edgeA and edgeB
        /// </summary>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Smoothstep(float edgeA, float edgeB, float x)
        {
            x = Clamp((x - edgeA) / (edgeB - edgeA), 0.0f, 1.0f);
            return x * x * (3 - 2 * x);
        }

        static float Noise(float x, float y)
        {
            int Noise2(int x1, int y1) =>
                hash[(hash[y1 % 256] + x1) % 256];

            float Smooth(float x1, float y1, float s1) =>
                Mathf.Lerp(x1, y1, s1 * s1 * (3 - 2 * s1));

            int x_int = (int)x, y_int = (int)y;
            float x_frac = x - x_int, y_frac = y - y_int;

            int s = Noise2(x_int, y_int);
            int t = Noise2(x_int + 1, y_int);
            int u = Noise2(x_int, y_int + 1);
            int v = Noise2(x_int + 1, y_int + 1);

            float low = Smooth(s, t, x_frac);
            float high = Smooth(u, v, x_frac);

            return Smooth(low, high, y_frac);
        }

        /// <summary>
        /// A Smooth Random Function
        /// </summary>
        public static float Perlin(float x, float y, float scale = 1, int depth = 4)
        {
            float xa = x * scale, ya = y * scale;
            float amp = 1.0f, fin = 0, div = 0.0f;

            for (int i = 0; i < depth; i++)
            {
                div += 256 * amp;
                fin += Noise(xa, ya) * amp;
                amp /= 2;
                xa *= 2;
                ya *= 2;
            }

            return fin / div;
        }
    }
}
