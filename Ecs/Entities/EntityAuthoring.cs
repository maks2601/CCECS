using System;
using UnityEngine;

namespace CursedCreatives.Ecs
{
    public class EntityAuthoring : MonoBehaviour
    {
        private bool _isInitialized;
        private Entity _entity;
        protected World _world;

        internal void Initialize(World world)
        {
            if (_isInitialized)
            {
                throw new Exception($"EntityProvider with id {_entity.Id} already initialized");
            }
            
            _world = world;
            _entity = world.CreateEntity(this);
            var providers = GetComponents<IComponentProvider>();
            foreach (IComponentProvider provider in providers)
            {
                provider.Initialize(_entity);
            }

            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            _isInitialized = true;
        }

        public Entity Entity
        {
            get
            {
                if (!_isInitialized)
                {
                    Debug.LogError($"no attached entity to {name}");
                }

                return _entity;
            }
        }
    }
}