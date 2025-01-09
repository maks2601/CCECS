#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace CursedCreatives.Ecs
{
    [CustomEditor(typeof(EntityAuthoring), true)]
    [CanEditMultipleObjects]
    public class EntityAuthoringEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying) DrawDefaultInspector();
        }
    }
}
#endif