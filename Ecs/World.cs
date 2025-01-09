using System;
using System.Collections;
using System.Collections.Generic;
using CursedCreatives.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CursedCreatives.Ecs
{
    public class World
    {
        private readonly List<SystemGroup> _systemGroups;
        private readonly Hashtable _componentPools;
        private readonly EntityPool _entityPool;
        private readonly EntityViewPool _userViews;
        private readonly List<Type> _componentTypes;
        private readonly WorldDisplay _worldDisplay;

        public World(string worldName = "World")
        {
            _systemGroups = new List<SystemGroup> { new UtilitySystemGroup(this) };

            _componentPools = new Hashtable();
            _entityPool = new EntityPool(this);
            _userViews = new EntityViewPool();

            _componentTypes = ReflectionUtility.FindAllDerivedClasses<IComponent>();
            foreach (Type type in _componentTypes)
            {
                _componentPools.Add(type, new ComponentPool());
            }

#if UNITY_EDITOR
            GameObject worldDisplayGO = new(worldName);
            _worldDisplay = worldDisplayGO.AddComponent<WorldDisplay>();
            _worldDisplay.Initialize(this, _systemGroups);
#endif
        }

        public void Initialize(EntityAuthoring entityAuthoring)
        {
            entityAuthoring.Initialize(this);
        }

        public T Instantiate<T>(T entityAuthoring) where T : EntityAuthoring
        {
            T instantiated = Object.Instantiate(entityAuthoring);
            instantiated.Initialize(this);
            return instantiated;
        }

        public T Instantiate<T>(T entityAuthoring, Transform parent) where T : EntityAuthoring
        {
            T instantiated = Object.Instantiate(entityAuthoring, parent);
            instantiated.Initialize(this);
            return instantiated;
        }

        internal ComponentPool GetPool(Type type)
        {
            return (ComponentPool)_componentPools[type];
        }

        public void CreateOneFrame(IComponent eventComponent)
        {
            Entity oneFrame = CreateEntity();
            oneFrame.Add(new OneFrameEvent { eventComponent = eventComponent });
        }

        public Entity CreateEntity()
        {
            Entity entity = GetEntityFromPool();
#if UNITY_EDITOR
            _worldDisplay.CreateEntityView(entity);
#endif
            return entity;
        }

        internal Entity CreateEntity(EntityAuthoring authoring)
        {
            Entity entity = GetEntityFromPool();
            AddEntityView(entity, authoring);
#if UNITY_EDITOR
            _worldDisplay.CreateEntityViewFromProvider(entity, authoring);
#endif
            return entity;
        }

        private void AddEntityView(Entity entity, EntityAuthoring authoring)
        {
            EntityView view = authoring.gameObject.AddComponent<EntityView>();
#if UNITY_EDITOR
            view.Initialize(this, entity, authoring);
#endif
            _userViews.Add(entity.Id, view);
        }

        private Entity GetEntityFromPool()
        {
            Entity entity = _entityPool.Create();
            foreach (ComponentPool pool in _componentPools.Values)
            {
                pool.TryAllocate(entity.Id);
            }

            return entity;
        }

        internal Entity GetEntity(int id)
        {
            return _entityPool.Get(id);
        }

        internal void RemoveEntity(int id)
        {
            foreach (ComponentPool pool in _componentPools.Values)
            {
                pool.RemoveComponent(id);
            }

            _userViews.Remove(id);
#if UNITY_EDITOR
            _worldDisplay.RemoveEntityView(id);
#endif
            _entityPool.Remove(id);
        }

        internal List<IComponent> GetEntityComponents(int id)
        {
            var components = new List<IComponent>();

            foreach (ComponentPool pool in _componentPools.Values)
            {
                if (pool.HasComponent(id))
                {
                    IComponent typeComponent = pool.GetComponent(id);
                    components.Add(typeComponent);
                }
            }

            return components;
        }

        public void AddSystemGroup(SystemGroup group)
        {
            _systemGroups.Add(group);
        }

        public void Update(float deltaTime)
        {
            foreach (SystemGroup systemGroup in _systemGroups)
            {
                systemGroup.Update(deltaTime);
            }
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            foreach (SystemGroup systemGroup in _systemGroups)
            {
                systemGroup.FixedUpdate(fixedDeltaTime);
            }
        }

        public void LateUpdate(float deltaTime)
        {
            foreach (SystemGroup systemGroup in _systemGroups)
            {
                systemGroup.LateUpdate(deltaTime);
            }
        }

        public void Destroy()
        {
            _entityPool.Clear();
#if UNITY_EDITOR
            Object.Destroy(_worldDisplay.gameObject);
#endif
        }

        internal List<Type> ComponentTypes => _componentTypes;
        
        internal EntityViewPool UserViews => _userViews;

        public Filter Filter => new(this);
    }
}