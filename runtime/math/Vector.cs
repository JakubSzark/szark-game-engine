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

        public float x, y;

        public Vector(float unit) :
            this(unit, unit)
        { }

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // -- Public Methods --

        /// <summary>
        /// The length of this vector
        /// </summary>
        public float Magnitude() =>
            (float)System.Math.Sqrt(x * x + y * y);

        /// <summary>
        /// The angle of this vector as a direction
        /// </summary>
        public float Angle() =>
            (float)System.Math.Atan2(x, y);

        /// <summary>
        /// Returns this vector with a magnitude of 1
        /// </summary>
        public Vector Normalized()
        {
            float mag = Magnitude();
            return new Vector(x, y) / (mag > 0 ? mag : 1);
        }

        /// <summary>
        /// Performs multiplication on each component
        /// of this vector based on the other vector's
        /// components.
        /// </summary>
        public Vector Scale(Vector other) =>
            new Vector(x * other.x, y * other.y);

        // -- Static Methods --

        /// <summary>
        /// Linearly interpolates a -> b based on the time.
        /// </summary>
        public static Vector Lerp(Vector a, Vector b, float time) =>
            new Vector(Mathf.Lerp(a.x, b.x, time), Mathf.Lerp(a.y, b.y, time));

        /// <summary>
        /// Returns the distance between a and b.
        /// </summary>
        public static float Distance(Vector a, Vector b) =>
            (float)System.Math.Sqrt((b.x - a.x) * (b.x - a.x) +
                (b.y - a.y) * (b.y - a.y));

        /// <summary>
        /// Returns the inner product of the two vectors.
        /// </summary>
        public static float Dot(Vector a, Vector b) =>
            a.x * b.x + a.y * b.y;

        // -- Overloaded Arithmetic Operators --

        public static Vector operator +(Vector a, Vector b) =>
            new Vector(a.x + b.x, a.y + b.y);

        public static Vector operator -(Vector a, Vector b) =>
            new Vector(a.x - b.x, a.y - b.y);

        public static Vector operator *(Vector a, float b) =>
            new Vector(a.x * b, a.y * b);

        public static Vector operator /(Vector a, float b) =>
            new Vector(a.x / b, a.y / b);

        public static Vector operator -(Vector a) =>
            new Vector(-a.x, -a.y);

        // -- Overloaded Equality Operators --

        public static bool operator ==(Vector a, Vector b) =>
            a.x == b.x && a.y == b.y;

        public static bool operator !=(Vector a, Vector b) =>
            a.x != b.x || a.y != b.y;

        // -- Equals, GetHashCode, and ToString

        public override bool Equals(object obj) =>
            obj is Vector vector && x == vector.x && y == vector.y;

        public override int GetHashCode() =>
            HashCode.Combine(x, y);

        public override string ToString() => $"({x},{y})";
    }
}