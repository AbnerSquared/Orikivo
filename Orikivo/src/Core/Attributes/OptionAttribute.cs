using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents an <see cref="Attribute"/> that marks a command with an option that can be specified to adjust how the command functions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OptionAttribute : Attribute
    {
        public OptionAttribute(Type type, string name, params string[] aliases)
        {
            Type = type;
            Name = name;
            Aliases = aliases;
        }

        public OptionAttribute(string name, params string[] aliases)
        {
            Type = null;
            Name = name;
            Aliases = aliases;
        }

        public Type Type { get; }
        public string Name { get; }
        public string Summary { get; }
        public IEnumerable<string> Aliases { get; }

        // the input given would have to be: --name value
        // -alias value
        public bool TryParse(string input, out object result)
        {
            result = null;
            var reader = new StringReader(input);

            bool isExplicit = false;
            bool isStart = true;
            bool isArray = true;

            string name = ""; // --NAME VALUE
            var values = new List<object>();

            var value = new StringBuilder(); // this is the builder for each value.

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (isStart)
                {
                    isStart = false;

                    if (c == '-')
                    {
                        if (reader.Peek() == '-')
                        {
                            isExplicit = true;
                            continue;
                        }
                    }

                    // If any of the aliases are specified, continue.
                    if (Aliases.Contains(reader.ReadString()))
                    {
                        if (Type == null)
                            return true;

                        continue;
                    }
                    else
                    {
                        return false;
                    }
                    
                }

                if (isExplicit)
                {
                    name = reader.ReadUnquotedString();

                    if (name != Name)
                        return false;
                    else
                    {
                        if (Type == null)
                            return true;

                        reader.SkipWhiteSpace(); // skip all of the whitespace between --NAME VALUE
                        continue;
                    }
                }

                if (isArray)
                {
                    if (c == ']') // This is the end of the value, close reader.
                    {
                        reader.Skip();
                        result = values;
                        return true;
                    }

                    if (c == ',') // This is a value separator, end and attach value.
                    {
                        if (TypeParser.TryParse(Type, value.ToString(), out result))
                        {
                            values.Add(result);
                            value.Clear();
                            continue;
                        }

                    }

                    value.Append(c);
                    continue;
                }

                if (reader.Peek() == '[') // This is the start of the value.
                {
                    isArray = true;
                    continue;
                }

                return TypeParser.TryParse(Type, reader.ReadString(), out result);
            }

            return false;
        }
    }
}
