using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orikivo.Text
{
    public abstract class ContentNode
    {
        protected virtual bool ReadAttributes { get; } = true;
        protected abstract string Formatting { get; }

        public override string ToString()
        {
            if (!ReadAttributes)
                return Formatting;

            List<NodeValue> nodes = GetNodeValues();
            var values = new object[nodes.Count];

            foreach(NodeValue node in nodes)
                values[node.Index] = node.Value;

            return values.Any() ? string.Format(Formatting, values) : Formatting;
        }

        private List<NodeValue> GetNodeValues()
        {
            var values = new List<NodeValue>();

            int i = 0;
            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                var indexer = property.GetCustomAttribute<NodeAttribute>();
                var formatting = property.GetCustomAttribute<FormattingAttribute>();
                var groupFormatting = property.GetCustomAttribute<GroupFormattingAttribute>();

                // if there is nothing that marks it as a node to use, skip it.
                if (indexer == null && formatting == null && groupFormatting == null)
                    continue;

                int index = indexer?.Index ?? i;
                object source = property.GetValue(this);

                string value = source?.ToString();

                if (value != null)
                {
                    if (groupFormatting != null)
                    {
                        if (!(source is IEnumerable<object> enumerable))
                            throw new InvalidCastException("The value specified could not be cast into an IEnumerable.");

                        string elements = string.Join(groupFormatting.Separator,
                            enumerable.Select(x => string.Format(groupFormatting.ElementFormat, x)));

                        value = string.Format(groupFormatting.Format, elements);
                    }
                    else if (formatting != null)
                    {
                        value = string.Format(formatting.Format, value);
                    }
                }

                values.Add(new NodeValue(index, value ?? ""));

                i++;
            }

            return values;
        }
    }
}
