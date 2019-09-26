using Orikivo.Storage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents an image resource for Orikivo.
    /// </summary>
    public struct Sprite
    {
        private Sprite(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("The specified path does not contain a value.");
            Source = path;
        }

        public static Sprite FromPath(string path)
            => new Sprite(path);

        public string Source { get; }

        public static implicit operator Bitmap(Sprite spr)
            => new Bitmap(spr);
    }

    public static class CacheIndex
    {
        public static FontCache Fonts { get; private set; }
        public static TokenCache Tokens { get; private set; }
        public static MeritCache Merits { get; private set; }

        static CacheIndex()
        {
            //Sprites = FileManager.GetSprites();
            Fonts = FileManager.GetFonts();
            Merits = FileManager.GetMerits();
        }

        public static void ReadAssembly(Assembly a)
        {
            StringBuilder sb = new StringBuilder();

            a.ExportedTypes.ForEach(x => sb.AppendLine(x.ToString()));
            a.GetManifestResourceNames().ForEach(x => sb.AppendLine(x));
            a.FullName.Debug("full name");
            a.CodeBase.Debug("code base");

            Manager.WriteTextAsync(sb.ToString(), ".//misc//tree.txt");
        }

        public static string GetEmbeddedResource(string resource, Assembly assembly)
        {
            resource = FormatResourceName(assembly, resource);

            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                if (resource == null)
                    return null;
                using (StreamReader reader = new StreamReader(resource))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static string FormatResourceName(Assembly assembly, string resource)
        {
            return $"{assembly.GetName().Name}.{resource.Replace(" ", "_").Replace("\\", ".").Replace("/", ".")}";
        }
    }
}
