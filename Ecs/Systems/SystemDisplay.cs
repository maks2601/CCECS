using Sirenix.OdinInspector;

namespace CursedCreatives.Ecs
{
    [HideReferenceObjectPicker]
    internal class SystemDisplay<T> where T : ISystem
    {
        private const string DISPLAY_GROUP_NAME = "Display";
        
        [HideLabel, HorizontalGroup(DISPLAY_GROUP_NAME)] 
        public bool enabled;
        
        [ReadOnly, LabelText("@_labelText"), HorizontalGroup(DISPLAY_GROUP_NAME)] 
        public T system;

        [ReadOnly, LabelText("ms"), HorizontalGroup(DISPLAY_GROUP_NAME)] 
        public double executionTime;
        
        private readonly string _labelText;

        public SystemDisplay(bool enabled, T system)
        {
            this.enabled = enabled;
            this.system = system;
            _labelText = system.GetType().Name;
        }
    }
}