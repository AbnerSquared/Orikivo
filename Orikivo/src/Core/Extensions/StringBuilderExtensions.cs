using System.Text;

namespace Orikivo
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends a specified number of copies of the specified string to this instance.
        /// </summary>
        public static StringBuilder Append(this StringBuilder stringBuilder, string value, int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
                stringBuilder.Append(value);

            return stringBuilder;
        }
    }
}
