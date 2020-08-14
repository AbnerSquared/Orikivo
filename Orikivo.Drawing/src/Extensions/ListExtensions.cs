using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    public static class ListExtensions
    {
        public static TResult CastObject<TResult>(this object obj)
        {
            if (obj is TResult result)
                return result;

            try
            {
                return (TResult) Convert.ChangeType(obj, typeof(TResult));
            }
            catch(InvalidCastException)
            {
                return default;
            }
        }
    }
}
