﻿using System;
using System.Collections.Generic;

#pragma warning disable CS1998

namespace Orikivo
{
    // TODO: Implement string being null. (ex. if someone types "null", you set the string to null)
    /// <summary>
    /// Represents a parser for <see cref="Type"/> values.
    /// </summary>
    public static class TypeParser
    {
        /// <summary>
        /// Represents the collection of custom parsers that are bound to a <see cref="Type"/>.
        /// </summary>
        private static Dictionary<Type, CustomTypeParser> Parsers
            => new Dictionary<Type, CustomTypeParser>
            {

            };

        public static void SetParser<T>(CustomTypeParser parser)
        {
            if (!Parsers.TryAdd(typeof(T), parser))
                Parsers[typeof(T)] = parser;
        }

        public static void SetParser(Type type, CustomTypeParser parser)
        {
            if (!Parsers.TryAdd(type, parser))
                Parsers[type] = parser;
        }

        public static bool TryParseSByte(string input, out sbyte result)
            => sbyte.TryParse(input, out result);

        public static bool TryParseInt16(string input, out short result)
            => short.TryParse(input, out result);

        public static bool TryParseInt32(string input, out int result)
            => int.TryParse(input, out result);

        public static bool TryParseInt64(string input, out long result)
            => long.TryParse(input, out result);

        public static bool TryParseByte(string input, out byte result)
            => byte.TryParse(input, out result);

        public static bool TryParseUInt16(string input, out ushort result)
            => ushort.TryParse(input, out result);

        public static bool TryParseUInt32(string input, out uint result)
            => uint.TryParse(input, out result);

        public static bool TryParseUInt64(string input, out ulong result)
            => ulong.TryParse(input, out result);

        public static bool TryParseBoolean(string input, out bool result)
            => bool.TryParse(input, out result);

        public static bool TryParseEnum<TEnum>(string input, out TEnum result)
            where TEnum : struct
            => Enum.TryParse(input, true, out result);

        public static bool TryParseEnum(Type enumType, string input, out object result)
            => Enum.TryParse(enumType, input, true, out result);

        // TODO: Find a way to condense this mess of code.
        // As of now, I can't exactly find a way to make this work, unless
        // I created a CustomTypeParser for each generic type.
        /// <summary>
        /// Attempts to parse the specified <see cref="Type"/>.
        /// </summary>
        public static bool TryParse(Type type, string input, out object result)
        {
            result = null;

            // Handle generics (string, int, etc.)
            if (type == typeof(string))
            {
                result = input;
                return true;
            }
            
            if (type == typeof(bool))
            {
                bool success = bool.TryParse(input, out var value);
                result = value;
                return success;
            }

            if (type == typeof(sbyte))
            {
                bool success = sbyte.TryParse(input, out var value);
                result = value;
                return success;
            }

            if (type == typeof(byte))
            {
                bool success = byte.TryParse(input, out var value);
                result = value;
                return success;
            }

            if (type == typeof(short))
            {
                bool success = short.TryParse(input, out var value);
                result = value;
                return success;
            }

            if (type == typeof(ushort))
            {
                bool success = ushort.TryParse(input, out var value);
                result = value;
                return success;
            }

            if (type == typeof(int))
            {
                bool success = int.TryParse(input, out var value);
                result = value;
                return success;
            }

            if (type == typeof(uint))
            {
                bool success = uint.TryParse(input, out var value);
                result = value;
                return success;
            }

            if (type == typeof(long))
            {
                bool success = long.TryParse(input, out var value);
                result = value;
                return success;
            }

            if (type == typeof(ulong))
            {
                bool success = ulong.TryParse(input, out var value);
                result = value;
                return success;
            }

            if (type.IsEnum)
            {
                return TryParseEnum(type, input, out result);
            }

            // and then handle customs
            if (Parsers.ContainsKey(type))
                return Parsers[type].TryParse(input, out result);

            return false;
        }
    }
}
