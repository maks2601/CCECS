using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace CursedCreatives.Ecs
{
    internal class WorldDisplay : SerializedMonoBehaviour
    {
#if UNITY_EDITOR
        [ListDrawerSettings(IsReadOnly = true, ShowFoldout = true), OdinSerialize]
        private List<SystemGroup> _systemGroups;

        private World _world;
        private EntityViewPool _worldViews;
        

        internal void Initialize(World world, List<SystemGroup> systemGroups)
        {
            DontDestroyOnLoad(gameObject);
            _systemGroups = systemGroups;
            _world = world;
            _worldViews = new EntityViewPool();
        }
        
        private void OnGUI()
        {
            _world.UserViews.Update();
            _worldViews.Update();
        }

        internal void CreateEntityViewFromProvider(Entity entity, EntityAuthoring authoring)
        {
            EntityView view = CreateEntityView(authoring.name);
            view.Initialize(_world, entity, authoring);
            _worldViews.Add(entity.Id, view);
        }
        
        internal void CreateEntityView(Entity entity)
        {
            EntityView view = CreateEntityView($"entity_{entity.Id}");
            view.Initialize(_world, entity);
            _worldViews.Add(entity.Id, view);
        }

        private EntityView CreateEntityView(string viewName)
        {
            GameObject viewGO = new(viewName);
            viewGO.transform.SetParent(transform);
            return viewGO.AddComponent<EntityView>();
        }

        internal void RemoveEntityView(int id)
        {
            _worldViews.Remove(id);
        }
#endif
    }
}