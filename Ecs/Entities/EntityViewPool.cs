using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CursedCreatives.Ecs
{
    internal class EntityViewPool
    {
        private readonly List<EntityView> _views;
        private readonly HashSet<int> _entities;

        internal EntityViewPool()
        {
            _views = new List<EntityView>();
            _entities = new HashSet<int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryAllocate(int id)
        {
            while (_views.Count <= id)
            {
                _views.Add(null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(int id, EntityView view)
        {
            TryAllocate(id);
            _views[id] = view;
            _entities.Add(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Remove(int id)
        {
            TryAllocate(id);
            if (_views[id] == null) return;

            Object.Destroy(_views[id].gameObject);
            _entities.Remove(id);
        }

#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Update()
        {
            _entities.RemoveWhere(id => _views[id] == null);

            foreach (int id in _entities)
            {
                _views[id].OnUpdate();
            }
        }
#endif
    }
}