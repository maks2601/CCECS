using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CursedCreatives.Ecs
{
    internal class ComponentPool
    {
        private readonly List<IComponent> _components;
        private readonly HashSet<int> _entities;

        internal ComponentPool()
        {
            _components = new List<IComponent>();
            _entities = new HashSet<int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void TryAllocate(int id)
        {
            if(_components.Count <= id) _components.Add(null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddComponent(int id, IComponent component)
        {
            if (HasComponent(id))
            {
                Debug.LogError($"Entity with id {id} already has component {component.GetType()}");
                return;
            }
            
            _components[id] = component;
            _entities.Add(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveComponent(int id)
        {
            _components[id] = null;
            _entities.Remove(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IComponent GetComponent(int id)
        {
            return _components[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasComponent(int id)
        {
            return _components[id] != null;
        }

        internal HashSet<int> Entities => _entities;
    }
}