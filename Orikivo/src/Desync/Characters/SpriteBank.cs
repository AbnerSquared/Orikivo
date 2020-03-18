using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a collection of <see cref="Sprite"/> values that can be called at any time.
    /// </summary>
    public class SpriteBank
    {
        public static SpriteBank FromDirectory(string directory)
        {
            SpriteBank bank = new SpriteBank();
            string[] sprites = Directory.GetFiles(directory, "*.png");

            if (sprites.Length == 0)
                throw new ArgumentException("The directory specified does not contain any sprites");
            
            foreach (string path in sprites)
            {
                bank.Import(new Sprite(path,
                    System.IO.Path.GetFileNameWithoutExtension(path)));
            }

            return bank;
        }

        public Dictionary<string, Sprite> Entries { get; set; } = new Dictionary<string, Sprite>();
        public void Import(Sprite sprite)
        {
            if (Entries.ContainsKey(sprite.Id))
                throw new ArgumentException("There is already a sprite stored using this ID.");

            Entries.Add(sprite.Id, sprite);
        }

        public Bitmap GetImage(string id)
        {
            return GetSprite(id).GetImage();
        }

        public Sprite GetSprite(string id)
        {
            if (!Entries.ContainsKey(id))
                throw new ArgumentException("There was no sprite that matched this ID.");
            return Entries[id];
        }
    }
}
