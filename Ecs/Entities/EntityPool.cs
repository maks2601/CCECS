using System.Collections.Generic;

namespace CursedCreatives.Ecs
{
    public class EntityPool
    {
        private readonly World _world;
        private readonly List<Entity> _entities;
        private readonly Queue<int> _releasedEntities;

        internal EntityPool(World world)
        {
            _world = world;
            _entities = new List<Entity>();
            _releasedEntities = new Queue<int>();
        }

        internal Entity Get(int id)
        {
            return _entities[id];
        }

        internal Entity Create()
        {
            Entity entity;

            if (_releasedEntities.TryPeek(out int id))
            {
                _releasedEntities.Dequeue();
                entity = new Entity(id, _world);
                _entities[id] = entity;
            }
            else
            {
                entity = new Entity(_entities.Count, _world);
                _entities.Add(entity);
            }

            return entity;
        }

        internal void Remove(int id)
        {
            _entities[id].Id = -1;
            _entities[id].IsDisposed = true;
            _entities[id] = null;
            _releasedEntities.Enqueue(id);
        }

        internal void Clear()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                if (_entities[i] == null) continue;
                _entities[i].Dispose();
            }
        }
    }
}