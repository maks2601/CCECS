using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CursedCreatives.Utilities;
using UnityEditor;

namespace CursedCreatives.Ecs
{
#if UNITY_EDITOR
    public static class MonoProviderGenerator
    {
        private const string TARGET_FOLDER = "Assets/Generated";

        [MenuItem("CursedCreatives/Generate Providers")]
        private static void OnScriptsReloaded()
        {
            if (!Directory.Exists(TARGET_FOLDER))
            {
                Directory.CreateDirectory(TARGET_FOLDER);
            }

            string[] guids = AssetDatabase.FindAssets("t:Script", new[] { TARGET_FOLDER });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null)
                {
                    Type type = script.GetClass();

                    if (type != null && !Attribute.IsDefined(type, typeof(GenerateProviderAttribute)))
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                    }
                }
            }

            var componentTypes = ReflectionUtility.FindAllDerivedClasses<IComponent>();
            var requireProviderTypes =
                componentTypes.Where(type => type.GetCustomAttribute<GenerateProviderAttribute>() != null);
            
            foreach (Type type in requireProviderTypes)
            {
                string providerCode = GenerateProvider(type);

                string filePath = Path.Combine(TARGET_FOLDER, type.Name + "Provider.cs");

                if (!File.Exists(filePath) && Attribute.IsDefined(type, typeof(GenerateProviderAttribute)))
                {
                    File.WriteAllText(filePath, providerCode);
                }
            }
            
            AssetDatabase.Refresh();
        }

        private static string GenerateProvider(Type type)
        {
            return $"using {type.Namespace};\n" +
                   "using UnityEngine;\n\n" +
                   "namespace CursedCreatives.Ecs.Generated\n" +
                   "{\n" +
                   $"\t[AddComponentMenu(\"ECS/{type.Name}\")]\n" +
                   $"\tpublic class {type.Name}Provider : MonoProvider<{type.Name}>\n" +
                   "\t{\n" +
                   "\t}\n" +
                   "}";
        }
    }
#endif
}