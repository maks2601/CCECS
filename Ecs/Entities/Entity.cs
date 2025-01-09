using System;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CursedCreatives.Ecs
{
    public class Entity
    {
        private readonly World _world;

        internal Entity(int id, World world)
        {
            Id = id;
            _world = world;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>() where T : IComponent
        {
            IComponent component = _world.GetPool(typeof(T)).GetComponent(Id);
            
            if (component == null)
            {
                throw new NullReferenceException($"no component of type {typeof(T)} on entity with id {Id}");
            }

            return (T)component;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>() where T : IComponent
        {
            return _world.GetPool(typeof(T)).HasComponent(Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<T>(T component) where T : IComponent
        {
            _world.GetPool(component.GetType()).AddComponent(Id, component);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Replace<T>(T component) where T : IComponent
        {
            Remove<T>();
            Add(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>() where T : IComponent
        {
            _world.GetPool(typeof(T)).RemoveComponent(Id);
        }

        public void Dispose()
        {
            _world.RemoveEntity(Id);
        }
        
        [ShowInInspector] public int Id { get; internal set; }
        public bool IsDisposed { get; internal set; }
    }
}