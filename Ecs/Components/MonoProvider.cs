using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CursedCreatives.Ecs
{
    public abstract class MonoProvider<T> : MonoBehaviour, IComponentProvider where T : IComponent
    {
        [SerializeField, HideReferenceObjectPicker, HideLabel]
        protected T _data;

        private void Reset()
        {
            _data ??= (T)Activator.CreateInstance(typeof(T));
        }

        public void Initialize(Entity entity)
        {
            entity.Add(_data);
            Destroy(this);
        }
    }
}