namespace CursedCreatives.Ecs
{
    public class OneFrameEvent : IComponent
    {
        public IComponent eventComponent;
        public bool isDirty;
    }
}