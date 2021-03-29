using System.Runtime.InteropServices;
using Szark.Math;

namespace Szark.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Shader
    {
        public readonly uint ID;
        public Shader(uint id) => ID = id;

        public uint? GetLocation(string name)
        {
            int val = Core.GetUniformLocation(ID, name);
            return val < 0 ? null : (uint)val;
        }

        public void Send(uint location, float value) =>
            Core.SendFloat(location, value);

        public void Send(uint location, Vec2 vec) =>
            Core.SendVec2(location, vec.X, vec.Y);

        public void Send(uint location, Vec3 vec) =>
            Core.SendVec3(location, vec.X, vec.Y, vec.Z);

        public void Send(uint location, Color color) =>
            Core.SendVec3(
                location,
                color.R / 255.0f,
                color.G / 255.0f,
                color.B / 255.0f
            );
    }
}