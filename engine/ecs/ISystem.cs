using Szark.Graphics;

namespace Szark.ECS
{
    /// <summary>
    /// The sole purpose of ISystem is to be implemented
    /// and used inside the entity manager to work upon
    /// components within ECS. Typically in the Execute
    /// method, We use Entities.ForEach to work over 
    /// all entities with specified components.
    /// </summary>
    public interface ISystem
    {
        void Execute(Canvas canvas, float deltaTime);
    }
}