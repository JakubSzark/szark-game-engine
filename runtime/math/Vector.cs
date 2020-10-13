using System;

namespace Szark.Math
{
    /// <summary>
    /// This class represents a mathematical vector
    /// of only two values. Also included is useful
    /// methods that are used in game developement.
    /// </summary>
    public partial struct Vector
    {
        public static readonly Vector One = new Vector(1, 1);
        public static readonly Vector Zero = new Vector(0, 0);

        public static readonly Vector Right = new Vector(1, 0);
        public static readonly Vector Left = new Vector(-1, 0);
        public static readonly Vector Down = new Vector(0, -1);
        public static readonly Vector Up = new Vector(0, 1);

        public float X, Y;

        public Vector(float unit) : this(unit, unit) { }
        public Vector(float x, float y) => (X, Y) = (x, y);

        // -- Public Methods --

        /// <summary>
        /// The length of this vector
        /// </summary>
        public float Magnitude() =>
            (float)System.Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// The angle of this vector as a direction
        /// </summary>
        public float Angle() =>
            (float)System.Math.Atan2(X, Y);

        /// <summary>
        /// Returns this vector with a magnitude of 1
        /// </summary>
        public Vector Normalized()
        {
            float mag = Magnitude();
            return new Vector(X, Y) / (mag > 0 ? mag : 1);
        }

        /// <summary>
        /// Performs multiplication on each component
        /// of this vector based on the other vector's
        /// components.
        /// </summary>
        public Vector Scale(Vector other) =>
            new Vector(X * other.X, Y * other.Y);

        // -- Static Methods --

        /// <summary>
        /// Linearly interpolates a -> b based on the time.
        /// </summary>
        public static Vector Lerp(Vector a, Vector b, float time) =>
            new Vector(Mathf.Lerp(a.X, b.X, time), Mathf.Lerp(a.Y, b.Y, time));

        /// <summary>
        /// Returns the distance between a and b.
        /// </summary>
        public static float Distance(Vector a, Vector b) =>
            (float)System.Math.Sqrt((b.X - a.X) * (b.X - a.X) +
                (b.Y - a.Y) * (b.Y - a.Y));

        /// <summary>
        /// Returns the inner product of the two vectors.
        /// </summary>
        public static float Dot(Vector a, Vector b) =>
            a.X * b.X + a.Y * b.Y;

        // -- Overloaded Arithmetic Operators --

        public static Vector operator +(Vector a, Vector b) =>
            new Vector(a.X + b.X, a.Y + b.Y);

        public static Vector operator -(Vector a, Vector b) =>
            new Vector(a.X - b.X, a.Y - b.Y);

        public static Vector operator *(Vector a, float b) =>
            new Vector(a.X * b, a.Y * b);

        public static Vector operator /(Vector a, float b) =>
            new Vector(a.X / b, a.Y / b);

        public static Vector operator -(Vector a) =>
            new Vector(-a.X, -a.Y);

        // -- Overloaded Equality Operators --

        public static bool operator ==(Vector a, Vector b) =>
            a.X == b.X && a.Y == b.Y;

        public static bool operator !=(Vector a, Vector b) =>
            a.X != b.X || a.Y != b.Y;

        // -- Equals, GetHashCode, and ToString

        public override bool Equals(object? obj) =>
            obj is Vector vector && X == vector.X && Y == vector.Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X},{Y})";
    }
}