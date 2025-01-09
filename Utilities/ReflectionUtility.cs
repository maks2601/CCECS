using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CursedCreatives.Utilities
{
    public static class ReflectionUtility
    {
        public static List<Type> FindAllDerivedClasses<T>()
        {
            var types = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var assemblyTypes = FindAllDerivedClasses<T>(assembly);
                foreach (Type type in assemblyTypes)
                {
                    types.Add(type);
                }
            }

            return types;
        }

        private static List<Type> FindAllDerivedClasses<T>(Assembly assembly)
        {
            Type baseType = typeof(T);
            return assembly
                .GetTypes()
                .Where(t =>
                    t != baseType &&
                    t.IsClass &&
                    baseType.IsAssignableFrom(t)
                ).ToList();
        }
    }
}