using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace Orikivo.Systems.Wrappers.Core.Converters
{
    public class ConvertUri
    {
        public static string ToUrlString(NameValueCollection collection, bool encoder = true)
        {
            if (encoder)
            {
                var list = (from key in collection.AllKeys
                    from value in collection.GetValues(key)
                    select $"{WebUtility.UrlDecode(key)}={WebUtility.UrlEncode(value)}").ToArray();
                return "?" + string.Join("&", list);
            }
            else
            {
                var list = (from key in collection.AllKeys
                    from value in collection.GetValues(key)
                    select $"{WebUtility.UrlEncode(key)}={value}").ToArray();
                return "?" + string.Join("&", list);
            }
        }
    }
}