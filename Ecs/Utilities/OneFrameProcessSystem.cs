namespace CursedCreatives.Ecs
{
    public class OneFrameProcessSystem : IUpdateSystem
    {
        private Filter _oneFrameFilter;

        public void Start()
        {
            _oneFrameFilter = World.Filter
                .With<OneFrameEvent>();
        }

        public void Update(float deltaTime)
        {
            foreach (Entity e in _oneFrameFilter.GetEntities())
            {
                OneFrameEvent oneFrameEvent = e.Get<OneFrameEvent>();
                if (oneFrameEvent.isDirty)
                {
                    e.Dispose();
                }
                else
                {
                    e.Add(oneFrameEvent.eventComponent);
                    oneFrameEvent.isDirty = true;
                }
            }
        }

        public World World { get; set; }
    }
}