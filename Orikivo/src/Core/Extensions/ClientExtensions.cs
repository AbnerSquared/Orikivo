using System;
using Orikivo.Converters;
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

        public static InteractionClientBuilder AddEnumTypeReader<T>(this InteractionClientBuilder builder)
            where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("The specified type is not a type of Enum");

            return builder.AddTypeReader<T>(new InteractionEnumTypeReader<T>());
        }

        public static InteractionClientBuilder AddEnumTypeConverter<T>(this InteractionClientBuilder builder)
            where T : struct, Enum
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("The specified type does not inherit the base type Enum");

            return builder.AddTypeConverter<T>(new EnumTypeConverter<T>());
        }
    }
}
