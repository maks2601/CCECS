namespace CursedCreatives.Ecs
{
    public class UtilitySystemGroup : SystemGroup
    {
        public UtilitySystemGroup(World world) : base(world)
        {
            AddUpdate(new OneFrameProcessSystem());
        }
    }
}