using System;
using System.Collections.Generic;
using System.Linq;

namespace CursedCreatives.Ecs
{
    public class Filter
    {
        private readonly World _world;
        private readonly List<Type> _includeTypes;
        private readonly List<Type> _excludeTypes;

        internal Filter(World world)
        {
            _world = world;
            _includeTypes = new List<Type>();
            _excludeTypes = new List<Type>();
        }

        public Filter With<T>() where T : IComponent
        {
            if (_excludeTypes.Contains(typeof(T)))
            {
                throw new Exception($"With and Without can't contain same types! Look for {typeof(T)}");
            }
            
            _includeTypes.Add(typeof(T));
            return this;
        }

        public Filter Without<T>()
        {
            if (_includeTypes.Contains(typeof(T)))
            {
                throw new Exception($"With and Without can't contain same types! Look for {typeof(T)}");
            }
            
            _excludeTypes.Add(typeof(T));
            return this;
        }

        public IEnumerable<Entity> GetEntities()
        {
            if (_includeTypes.Count == 0)
            {
                throw new Exception("Filter must contain With!");
            }

            Type smallestPoolType = GetSmallestPoolType(_includeTypes);
            var entities = new HashSet<int>(_world.GetPool(smallestPoolType).Entities);

            foreach (Type type in _includeTypes)
            {
                if(type == smallestPoolType) continue;
                
                IntersectPool(entities, _world.GetPool(type));
            }

            foreach (Type type in _excludeTypes)
            {
                ExceptPool(entities, _world.GetPool(type));
            }

            return entities.Select(id => _world.GetEntity(id));
        }

        private Type GetSmallestPoolType(List<Type> types)
        {
            int smallestCount = (int)1e9;
            Type smallestPoolType = types.First();

            foreach (Type type in types)
            {
                var pool = _world.GetPool(type).Entities;

                if (pool.Count >= smallestCount) continue;

                smallestCount = pool.Count;
                smallestPoolType = type;
            }

            return smallestPoolType;
        }

        private void IntersectPool(HashSet<int> current, ComponentPool other)
        {
            current.RemoveWhere(id => !other.HasComponent(id));
        }
        
        private void ExceptPool(HashSet<int> current, ComponentPool other)
        {
            current.RemoveWhere(other.HasComponent);
        }
    }
}