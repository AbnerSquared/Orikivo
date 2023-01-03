using System.Collections.Generic;
using Orikivo;

namespace Arcadia
{
    public sealed class TextList : TextSection
    {
        public string Bullet { get; set; }

        public List<string> Values { get; set; } = new List<string>();

        /// <inheritdoc />
        public override string Content => Format.List(Values, Bullet);
        
        // The number of spaces to apply for each value in a list.
        public int Depth { get; set; } = 0;
    }
}

/*
    NOTE: It might be possible to add true depth for elements in a list.

    List 1:
      - Value A
      - Value B
      - List 2:
          - Value A
          - Value B


    When building text bodies, handle writing like this:




     
     */