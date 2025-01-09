namespace CursedCreatives.Ecs
{
    public interface ILateSystem : ISystem
    {
        public void LateUpdate(float deltaTime);
    }
}