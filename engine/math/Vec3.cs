using System;
using System.Runtime.CompilerServices;

namespace Szark.Math
{
    /// <summary>
    /// This class represents a mathematical vector
    /// of three values. Useful for 3D operations
    /// </summary>
    public partial struct Vec3
    {
        public float X, Y, Z;

        public Vec3(float unit) : this(unit, unit, unit) { }
        public Vec3(float x, float y, float z) => (X, Y, Z) = (x, y, z);
        public Vec3(float x, float y) => (X, Y, Z) = (x, y, 1.0f);
        public Vec3(Vec2 vec) => (X, Y, Z) = (vec.X, vec.Y, 1.0f);

        // -- Constants --

        public static readonly Vec3 One = new Vec3(1, 1, 1);
        public static readonly Vec3 Zero = new Vec3(0, 0, 0);

        public static readonly Vec3 Right = new Vec3(1, 0, 0);
        public static readonly Vec3 Left = new Vec3(-1, 0, 0);
        public static readonly Vec3 Down = new Vec3(0, -1, 0);
        public static readonly Vec3 Up = new Vec3(0, 1, 0);
        public static readonly Vec3 Forward = new Vec3(0, 0, 1);
        public static readonly Vec3 Back = new Vec3(0, 0, -1);

        // -- Public Methods --

        /// <summary>
        /// The length of this vector
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude() =>
            (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);

        /// <summary>
        /// Returns this vector with a magnitude of 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec3 Normalized()
        {
            float mag = Magnitude();
            return new Vec3(X, Y, Z) / (mag > 0 ? mag : 1);
        }

        /// <summary>
        /// Performs multiplication on each component
        /// of this vector based on the other vector's
        /// components.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec3 Scale(Vec3 other) =>
            new Vec3(X * other.X, Y * other.Y, Z * other.Z);

        /// <summary>
        /// Returns a perpendicular vector from given.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec3 GetPerpendicular()
        {
            return System.Math.Abs(Z) < System.Math.Abs(X) ?
                new Vec3(Y, -X, 0) : new Vec3(0, -Z, Y);
        }

        /// <summary>
        /// Returns the Cross Product of Two Vectors
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            return new Vec3()
            {
                X = a.Y * b.Z - b.Y * a.Z,
                Y = (a.X * b.Z - b.X * a.Z) * -1,
                Z = a.X * b.Y - b.X * a.Y,
            };
        }

        /// <summary>
        /// Deconstruct this Vector into its Components 
        /// </summary>
        public void Deconstruct(out float X, out float Y, out float Z)
        {
            X = this.X;
            Y = this.Y;
            Z = this.Z;
        }

        // -- Static Methods --

        /// <summary>
        /// Linearly interpolates a -> b based on the time.
        /// </summary>
        public static Vec3 Lerp(Vec3 a, Vec3 b, float time) =>
            new Vec3(Mathf.Lerp(a.X, b.X, time), Mathf.Lerp(a.Y, b.Y, time),
                Mathf.Lerp(a.Z, a.Z, time));

        /// <summary>
        /// Returns the distance between a and b.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vec3 a, Vec3 b) =>
            (float)System.Math.Sqrt((b.X - a.X) * (b.X - a.X) +
                (b.Y - a.Y) * (b.Y - a.Y) + (b.Z - a.Z) * (b.Z - a.Z));

        /// <summary>
        /// Returns the inner product of the two vectors.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vec3 a, Vec3 b) =>
            (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);

        // -- Overloaded Arithmetic Operators --

        public static Vec3 operator +(Vec3 a, Vec3 b) =>
            new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vec3 operator -(Vec3 a, Vec3 b) =>
            new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vec3 operator *(Vec3 a, float b) =>
            new Vec3(a.X * b, a.Y * b, a.Z * b);

        public static Vec3 operator /(Vec3 a, float b) =>
            new Vec3(a.X / b, a.Y / b, a.Z / b);

        public static Vec3 operator -(Vec3 a) =>
            new Vec3(-a.X, -a.Y, -a.Z);

        public static Vec3 operator %(Vec3 a, Vec3 b) =>
            Vec3.Cross(a, b);

        // -- Overloaded Equality Operators --

        public static bool operator ==(Vec3 a, Vec3 b) =>
            a.X == b.X && a.Y == b.Y && a.Z == b.Z;

        public static bool operator !=(Vec3 a, Vec3 b) =>
            a.X != b.X || a.Y != b.Y || a.Z != b.Z;

        // -- Equals, GetHashCode, and ToString

        public override bool Equals(object? obj) =>
            obj is Vec3 vector && X == vector.X && Y == vector.Y && Z == vector.Z;

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);
        public override string ToString() => $"({X},{Y},{Z})";
    }
}