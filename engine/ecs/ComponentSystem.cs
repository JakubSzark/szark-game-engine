using Szark.Graphics;

namespace Szark.ECS
{
    /// <summary>
    /// Abstract class for creating systems. This class provides 
    /// properties to easily interact with your game. You can technically
    /// still use ISystem instead for a lighterweight option.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ComponentSystem<T> : ISystem
        where T : Szark.Game
    {
        protected T Game => Szark.Game.Get<T>();
        protected EntityManager EntityManager => Game.EntityManager;

        /// <summary>
        /// Returns a component on an entity
        /// </summary>
        protected C? GetComponent<C>(Entity entity)
            where C : struct, IComponent =>
            EntityManager.GetComponent<C>(entity);

        /// <summary>
        /// Adds a component to an entity
        /// </summary>
        protected void AddComponent<C>(Entity entity, C comp)
            where C : struct, IComponent =>
            EntityManager.AddComponent<C>(entity, comp);

        /// <summary>
        /// Removes a component from an entity
        /// </summary>
        protected void RemoveComponent<C>(Entity entity)
            where C : struct, IComponent =>
            EntityManager.RemoveComponent<C>(entity);

        public abstract void Execute(Canvas canvas, float deltaTime);
    }
}