namespace CursedCreatives.Ecs
{
    public interface IFixedSystem : ISystem
    {
        public void FixedUpdate(float fixedDeltaTime);
    }
}