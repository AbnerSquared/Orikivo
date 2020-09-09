using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Arcadia.Formatters
{
    public class CurrencyFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string argFmt = string.Empty;

            if (!string.IsNullOrEmpty(format))
                argFmt = format.Length > 1 ? format[..1] : format;

            if (!(arg is long value))
            {
                return HandleOtherFormats(format, arg);
            }

            return argFmt switch
            {
                "o" => $"{Icons.Balance} **{value:##,0}**",
                "O" => $"**Balance**: {Icons.Balance} **{value:##,0}**",
                "c" => $"{Icons.Chips} **{value:##,0}**",
                "C" => $"**Chips**: {Icons.Chips} **{value:##,0}**",
                "t" => $"{Icons.Tokens} **{value:##,0}**",
                "T" => $"**Tokens**: {Icons.Tokens} **{value:##,0}**",
                "d" => $"{Icons.Debt} **{value:##,0}**",
                "D" => $"**Debt**: {Icons.Debt} **{value:##,0}**",
                _ => HandleOtherFormats(format, arg)
            };
        }

        private string HandleOtherFormats(string format, object arg)
        {
            if (arg is IFormattable formattable)
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            else if (arg != null)
                return arg.ToString();
            else
                return string.Empty;
        }
    }
}
