using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Static
{
    public enum ReactionType
    {
        Owo = 1
    }

    public static class ImageIndex
    {
        private static string GetPath(ReactionType type)
        {
            string path = "";
            switch(type)
            {
                case ReactionType.Owo:
                    path = "https://orig00.deviantart.net/bc18/f/2018/140/2/1/orikivo_iii____animated_emoticons_owo_by_abnersquared-dbtju89.gif";
                    break;
                default:
                    throw new Exception("Invalid reaction type.");
            }
            return path;
        }

        // Make reactions a dynamic object, like how fonts are.
        // Build support for checking if they can use a reaction type.
        public static string React(ReactionType type)
            => GetPath(type);
    }
}
