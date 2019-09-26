using System.Text;

namespace Orikivo
{
    public static class StringBuilderExtender
    {
        public static void AppendLines(this StringBuilder sb, params string[] args)
        {
            foreach (string s in args)
            {
                sb.AppendLine(s);
            }
        }
    }
}