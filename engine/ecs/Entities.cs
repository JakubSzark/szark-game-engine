using System;
using System.Collections.Generic;

namespace Szark.ECS
{
    public class QueryBuilder
    {
        // Delegates for ForEach Methods
        public delegate void CompAction<T>(ref T t);
        public delegate void CompAction<T, J, K>(ref T t, ref J j, ref K k);
        public delegate void CompAction<T, J>(ref T t, ref J j);

        internal List<Type> Requirements;
        internal QueryBuilder() { Requirements = new List<Type>(); }

        public QueryBuilder WithTag<T>() where T : struct, ITag
        {
            Requirements.Add(typeof(T));
            return this;
        }

        private BytePool? GetPool<T>() where T : struct, IComponent =>
            GetPool(typeof(T));

        private BytePool? GetPool(Type type)
        {
            var components = Game.Get<Game>().EntityManager.components;
            components.TryGetValue(type, out BytePool? pool);
            return pool;
        }

        private bool DoesMeetRequirementsAt(int index)
        {
            foreach (var req in Requirements)
            {
                if (!(GetPool(req) is BytePool pool && pool.IsValidAt(index)))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Loops through entities with the matching component
        /// </summary>
        public void ForEach<T>(CompAction<T> action)
            where T : struct, IComponent
        {
            var manager = Game.Get<Game>().EntityManager;
            var components = manager.components;

            if (GetPool<T>() is BytePool pool)
            {
                for (int i = 0; i < manager.entities.Count; i++)
                {
                    if (DoesMeetRequirementsAt(i) && pool.IsValidAt(i))
                    {
                        var spanT = pool.AsSpan<T>();
                        action(ref spanT[i]);
                    }
                }
            }

            Requirements.Clear();
        }

        /// <summary>
        /// Loops through entities with matching two components
        /// </summary>
        public void ForEach<T, J>(CompAction<T, J> action)
            where T : struct, IComponent where J : struct, IComponent
        {
            var manager = Game.Get<Game>().EntityManager;
            var components = manager.components;

            if (GetPool<T>() is BytePool poolT)
            {
                if (GetPool<J>() is BytePool poolJ)
                {
                    for (int i = 0; i < manager.entities.Count; i++)
                    {
                        if (DoesMeetRequirementsAt(i) && poolT.IsValidAt(i) && poolJ.IsValidAt(i))
                        {
                            var spanT = poolT.AsSpan<T>();
                            var spanJ = poolJ.AsSpan<J>();
                            action(ref spanT[i], ref spanJ[i]);
                        }
                    }
                }
            }

            Requirements.Clear();
        }

        /// <summary>
        /// Loops through entities with matching three components
        /// </summary>
        public void ForEach<T, J, K>(CompAction<T, J, K> action)
            where T : struct, IComponent
            where J : struct, IComponent
            where K : struct, IComponent
        {
            var manager = Game.Get<Game>().EntityManager;
            var components = manager.components;

            if (GetPool<T>() is BytePool poolT)
            {
                if (GetPool<J>() is BytePool poolJ)
                {
                    if (GetPool<K>() is BytePool poolK)
                    {
                        for (int i = 0; i < manager.entities.Count; i++)
                        {
                            if (DoesMeetRequirementsAt(i) && poolT.IsValidAt(i) &&
                                poolJ.IsValidAt(i) && poolK.IsValidAt(i))
                            {
                                var spanT = poolT.AsSpan<T>();
                                var spanJ = poolJ.AsSpan<J>();
                                var spanK = poolK.AsSpan<K>();

                                action(ref spanT[i], ref spanJ[i], ref spanK[i]);
                            }
                        }
                    }
                }
            }

            Requirements.Clear();
        }
    }

    public static class Entities
    {
        private static QueryBuilder builder = new QueryBuilder();
        public static QueryBuilder Query() => builder;
    }
}
