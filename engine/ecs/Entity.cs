using System;

namespace Szark.ECS
{
    /// <summary>
    /// The building piece / object of ECS
    /// </summary>
    public struct Entity : IComparable<Entity>
    {
        // Represents an Non-Existent Entity
        public static readonly Entity None = new Entity(-1);

        public readonly int ID;
        public Entity(int id) => ID = id;

        public int CompareTo(Entity other) =>
            ID - other.ID;

        public override bool Equals(object? obj) =>
            obj is Entity entity && base.Equals(entity);

        public override int GetHashCode() =>
            HashCode.Combine(ID);

        public static bool operator ==(Entity left, Entity right) =>
            left.ID == right.ID;

        public static bool operator !=(Entity left, Entity right) =>
            left.ID != right.ID;
    }
}
