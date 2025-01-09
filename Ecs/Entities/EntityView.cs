using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;

namespace CursedCreatives.Ecs
{
    public class EntityView : SerializedMonoBehaviour
    {
#if UNITY_EDITOR
        private const string ADD_COMPONENT_GROUP = "Add Component";
        private const string REMOVE_FUNCTION = "RemoveComponent";
        private const string COMPONENT_VALIDATION_FUNCTION = "AddComponentValidation";
        private const string SET_COMPONENT_FUNCTION = "SetSelectedComponent";
        private const string GET_TYPE_FUNCTION = "GetAvailableTypes";
        
        [ShowInInspector, ReadOnly] private EntityAuthoring _entityAuthoring;
        [ShowInInspector, ReadOnly] private int _id = -1;
        
        [Serializable]
        internal struct ComponentPreview
        {
            private readonly string _typeName;

            [HideReferenceObjectPicker, LabelText("@_typeName")]
            public IComponent component;

            internal ComponentPreview(IComponent component)
            {
                this.component = component;

                string name = component.GetType().Name;
                _typeName = name.Replace("Component", "");
            }
        }

        [BoxGroup(ADD_COMPONENT_GROUP)]
        [ShowInInspector]
        [OnValueChanged(SET_COMPONENT_FUNCTION)]
        [ValueDropdown(GET_TYPE_FUNCTION)]
        private Type _type;
        
        [BoxGroup(ADD_COMPONENT_GROUP)]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        [HideLabel]
        [OdinSerialize]
        private IComponent _component;

        private Entity _entity;
        private World _world;
        
        [ShowInInspector, OdinSerialize]
        [ListDrawerSettings(
            ShowItemCount = true,
            CustomRemoveIndexFunction = REMOVE_FUNCTION,
            HideAddButton = true,
            DraggableItems = false,
            ShowFoldout = true)
        ]
        private List<ComponentPreview> _components = new();

        public void Initialize(World world, Entity entity, EntityAuthoring authoring = null)
        {
            _world = world;
            _entity = entity;
            _entityAuthoring = authoring;
            _id = _entity.Id;
        }
        
        [BoxGroup(ADD_COMPONENT_GROUP)]
        [EnableIf(COMPONENT_VALIDATION_FUNCTION)]
        [Button("Add")]
        private void AddComponent()
        {
            _world.GetPool(_component.GetType()).AddComponent(_entity.Id, _component);

            SetSelectedComponent();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnUpdate()
        {
            if (Selection.activeGameObject != gameObject) return;

            var componentsTemp = new List<ComponentPreview>();

            foreach (IComponent component in _world.GetEntityComponents(_entity.Id))
            {
                componentsTemp.Add(new ComponentPreview(component));
            }

            _components = componentsTemp;
        }
        
        private void SetSelectedComponent() => _component = (IComponent)Activator.CreateInstance(_type);

        private bool AddComponentValidation()
        {
            return _component != null &&
                   _components.All(componentPreview => componentPreview.component.GetType() != _type);
        }
        
        private IEnumerable<Type> GetAvailableTypes()
        {
            var usedValues = _components.Select(componentPreview => componentPreview.component.GetType()).ToList();

            var availableValues = _world.ComponentTypes.Except(usedValues);

            Type firstAvailableValue = availableValues.First();

            _type = firstAvailableValue;
            SetSelectedComponent();

            return availableValues;
        }
        
        private void RemoveComponent(int index)
        {
            IComponent removed = _components[index].component;
            _world.GetPool(removed.GetType()).RemoveComponent(_entity.Id);
        }

        private void OnValidate()
        {
            _components.Clear();
        }
#endif
    }
}