using System;
using Orikivo.Framework;

namespace Orikivo
{
    public static class ClientExtensions
    {
        /// <summary>
        /// Adds a new <see cref="EnumTypeReader{TEnum}"/> for ths specified <see cref="Enum"/> to the <see cref="ClientBuilder"/>.
        /// </summary>
        public static ClientBuilder AddEnumTypeReader<T>(this ClientBuilder builder)
            where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("The specified type is not a type of Enum");

            return builder.AddTypeReader<T>(new EnumTypeReader<T>());
        }
    }
}
