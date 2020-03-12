using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orikivo.Text
{
    public abstract class ContentNode
    {
        protected abstract string Formatting { get; }
        public override string ToString()
        {
            List<NodeValue> nodes = GetNodeValues();
            string[] values = new string[nodes.Count];

            foreach(NodeValue node in nodes)
                values[node.Index] = node.Value;

            return values.Count() > 0 ? string.Format(Formatting, values) : Formatting;
        }

        private List<NodeValue> GetNodeValues()
        {
            List<NodeValue> values = new List<NodeValue>();

            int i = 0;
            PropertyInfo[] properties = GetType().GetProperties();
            
            foreach (PropertyInfo property in properties)
            {
                NodeAttribute indexer = property.GetAttribute<NodeAttribute>();
                FormattingAttribute formatting = property.GetAttribute<FormattingAttribute>();
                GroupFormattingAttribute groupFormatting = property.GetAttribute<GroupFormattingAttribute>();

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
                        if (!(source is IEnumerable))
                            throw new InvalidCastException("The value specified could not be cast into an IEnumerable.");

                        value = string.Format(groupFormatting.Format,
                            string.Join(groupFormatting.Separator,
                            ((IEnumerable)source).OfType<object>().Select(x => string.Format(groupFormatting.ElementFormat, x))));
                    }
                    else if (formatting != null)
                        value = string.Format(formatting.Format, value);
                }

                values.Add(new NodeValue(index, value ?? ""));

                i++;
            }

            return values;
        }
    }
}
