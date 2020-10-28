using System.Collections.Generic;
using Orikivo;

namespace Arcadia
{
    public class TextList : TextSection
    {
        public string Bullet { get; set; }

        public List<string> Values { get; set; } = new List<string>();

        /// <inheritdoc />
        public override string Content => Format.List(Values, Bullet);
    }
}
