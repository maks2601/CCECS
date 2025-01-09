namespace CursedCreatives.Ecs
{
    public interface ISystem
    {
        public void Start();

        public World World { get; set; }
    }
}