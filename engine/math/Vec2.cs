using System;
using System.Runtime.CompilerServices;

namespace Szark.Math
{
    /// <summary>
    /// This class represents a mathematical vector
    /// of only two values. Also included is useful
    /// methods that are used in game developement.
    /// </summary>
    public partial struct Vec2
    {
        public float X, Y;

        public Vec2(float unit) : this(unit, unit) { }
        public Vec2(float x, float y) => (X, Y) = (x, y);

        // -- Constants --

        public static readonly Vec2 One = new Vec2(1, 1);
        public static readonly Vec2 Zero = new Vec2(0, 0);

        public static readonly Vec2 Right = new Vec2(1, 0);
        public static readonly Vec2 Left = new Vec2(-1, 0);
        public static readonly Vec2 Down = new Vec2(0, -1);
        public static readonly Vec2 Up = new Vec2(0, 1);

        // -- Public Methods --

        /// <summary>
        /// The length of this vector
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude() =>
            (float)System.Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// The angle of this vector as a direction
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Angle() =>
            (float)System.Math.Atan2(X, Y);

        /// <summary>
        /// Returns this vector with a magnitude of 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec2 Normalized()
        {
            float mag = Magnitude();
            return new Vec2(X, Y) / (mag > 0 ? mag : 1);
        }

        /// <summary>
        /// Performs multiplication on each component
        /// of this vector based on the other vector's
        /// components.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec2 Scale(Vec2 other) =>
            new Vec2(X * other.X, Y * other.Y);

        /// <summary>
        /// Deconstruct this Vector into its Components 
        /// </summary>
        public void Deconstruct(out float X, out float Y)
        {
            X = this.X;
            Y = this.Y;
        }

        // -- Static Methods --

        /// <summary>
        /// Linearly interpolates a -> b based on the time.
        /// </summary>
        public static Vec2 Lerp(Vec2 a, Vec2 b, float time) =>
            new Vec2(Mathf.Lerp(a.X, b.X, time), Mathf.Lerp(a.Y, b.Y, time));

        /// <summary>
        /// Returns the distance between a and b.
        /// </summary>
        public static float Distance(Vec2 a, Vec2 b) =>
            (float)System.Math.Sqrt((b.X - a.X) * (b.X - a.X) +
                (b.Y - a.Y) * (b.Y - a.Y));

        /// <summary>
        /// Returns the inner product of the two vectors.
        /// </summary>
        public static float Dot(Vec2 a, Vec2 b) =>
            a.X * b.X + a.Y * b.Y;

        // -- Overloaded Arithmetic Operators --

        public static Vec2 operator +(Vec2 a, Vec2 b) =>
            new Vec2(a.X + b.X, a.Y + b.Y);

        public static Vec2 operator -(Vec2 a, Vec2 b) =>
            new Vec2(a.X - b.X, a.Y - b.Y);

        public static Vec2 operator *(Vec2 a, float b) =>
            new Vec2(a.X * b, a.Y * b);

        public static Vec2 operator /(Vec2 a, float b) =>
            new Vec2(a.X / b, a.Y / b);

        public static Vec2 operator -(Vec2 a) =>
            new Vec2(-a.X, -a.Y);

        // -- Overloaded Equality Operators --

        public static bool operator ==(Vec2 a, Vec2 b) =>
            a.X == b.X && a.Y == b.Y;

        public static bool operator !=(Vec2 a, Vec2 b) =>
            a.X != b.X || a.Y != b.Y;

        // -- Equals, GetHashCode, and ToString

        public override bool Equals(object? obj) =>
            obj is Vec2 vector && X == vector.X && Y == vector.Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X},{Y})";
    }
}