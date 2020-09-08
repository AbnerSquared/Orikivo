using System;

namespace Orikivo.Text.Pagination
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

        public string DefaultElement { get; set; }

        public int? CharacterLimit { get; set; }
    }
}