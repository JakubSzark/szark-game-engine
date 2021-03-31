namespace Szark.ECS
{
    /// <summary>
    /// Data attached to every entity.
    /// </summary>
    public interface IComponent { }

    /// <summary>
    /// An ITag is a component for the purpose 
    /// of only tagging entities.
    /// </summary>
    public interface ITag : IComponent { }
}