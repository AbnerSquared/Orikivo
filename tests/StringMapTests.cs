using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public class StringMapTests
    {
        public static string GetNodeContent(string content, string title = null, string frame = null)
        {
            StringNode node = new StringNode();
            node.Title = title;
            node.Content = content;
            node.PropertyMap = frame;
            Console.WriteLine("[Debug] -- Node built. --");
            return node.ToString();
        }
    }
}
