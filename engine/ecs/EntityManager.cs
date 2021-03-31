using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System;
using Szark.Graphics;

namespace Szark.ECS
{
    public class EntityManager
    {
        /// <summary>
        /// Returns the amount of entities
        /// </summary>
        public int Count => entities.Count;

        internal List<ISystem> systems;
        internal Dictionary<Type, BytePool> components;
        internal List<Entity> entities;

        internal EntityManager(Assembly assembly)
        {
            entities = new List<Entity>();
            systems = new List<ISystem>();
            components = new Dictionary<Type, BytePool>();

            // Find all types in the current assembly
            var types = assembly?.GetTypes();

            // Get all systems from types list
            if (types == null) return;
            foreach (var type in types)
            {
                if (!type.IsInterface && typeof(ISystem).IsAssignableFrom(type))
                {
                    // Instantiate all systems found in assembly
                    var instance = (ISystem?)Activator.CreateInstance(type);
                    if (instance != null) systems.Add(instance);
                }
            }
        }

        /// <summary>
        /// Executes all systems with the execution type
        /// </summary>
        public void ExecuteSystems(Canvas canvas, float deltaTime) =>
            systems.ForEach(s => s.Execute(canvas, deltaTime));

        /// <summary>
        /// Creates an entity or fills in already existing spaces.
        /// Also does the same for each component given.
        /// </summary>
        public Entity CreateEntity()
        {
            // Check for empty space
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] == Entity.None)
                {
                    entities[i] = new Entity(i);
                    return entities[i];
                }
            }

            var result = new Entity(entities.Count);
            entities.Add(result);

            // Add onto each component pool
            foreach (var pair in components)
                pair.Value.Push();

            return result;
        }

        /// <summary>
        /// Adds a component to an entity
        /// </summary>
        public void AddComponent<T>(Entity entity, T component)
            where T : struct, IComponent
        {
            if (!components.ContainsKey(typeof(T)))
                components.Add(typeof(T), new BytePool(Marshal.SizeOf<T>()));
            components[typeof(T)].Assign(component, entity.ID);
        }

        /// <summary>
        /// Adds a tag to the entity.
        /// This is like add component but more restricted to ITag's
        /// </summary>
        public void AddTag<T>(Entity entity, T tag) where T : struct, ITag =>
            AddComponent(entity, tag);

        /// <summary>
        /// Returns a component from an entity
        /// </summary>
        public T? GetComponent<T>(Entity entity) where T : struct, IComponent
        {
            if (components.ContainsKey(typeof(T)))
            {
                var poolResult = components[typeof(T)].Get<T>(entity.ID);
                if (poolResult.IsValid) return poolResult.Value;
            }

            return null;
        }

        /// <summary>
        /// Removes a component to an entity
        /// </summary>
        public void RemoveComponent<T>(Entity entity)
            where T : struct, IComponent
        {
            if (components.ContainsKey(typeof(T)))
                components[typeof(T)].Invalidate(entity.ID);
        }

        /// <summary>
        /// Removes an entity from the entities list.
        /// And clears associated components to null.
        /// </summary>
        public void DestroyEntity(Entity entity)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].CompareTo(entity) == 0)
                {
                    entities[i] = Entity.None;
                    foreach (var pair in components)
                        pair.Value.Invalidate(i);
                }
            }
        }
    }
}
