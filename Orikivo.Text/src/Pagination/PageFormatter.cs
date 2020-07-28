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
            get
            {
                if (_elementFormatter == null)
                    return x => x.ToString();

                return _elementFormatter;
            }

            set => _elementFormatter = value;
        }

        public int? CharacterLimit { get; set; }
    }
}