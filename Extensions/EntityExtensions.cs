using System.Collections.Generic;
using CursedCreatives.Ecs;

namespace CursedCreatives.Extensions
{
    public static class EntityExtensions
    {
        public static bool IsNullOrDisposed(this Entity entity)
        {
            return entity == null || entity.IsDisposed;
        }
        
        public static void RemoveNulls(this IList<Entity> list)
        {
            for (int i = list.Count - 1; i >= 0 ; i--)
            {
                if (list[i].IsNullOrDisposed())
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}