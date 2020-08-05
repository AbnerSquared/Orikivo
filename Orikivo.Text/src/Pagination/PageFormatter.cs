using System;

namespace Orikivo.Text
{
    public class PageFormatter<T>
    {
        public string BaseFormatter { get; set; }

        public string Separator { get; set; }

        private Func<T, string> _elementFormatter;

        public Func<T, string> ElementFormatter
        {
            get => _elementFormatter ?? (x => x.ToString());
            set => _elementFormatter = value;
        }

        public int? CharacterLimit { get; set; }
    }
}