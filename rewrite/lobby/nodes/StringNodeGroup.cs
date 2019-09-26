using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public enum CapacityOverflowAction
    {
        Exception = 1, // an error is thrown when trying to add another item to the group
        Push = 2, // the oldest one is pushed out, with the new one being inserted.
        Clear = 3, // the list is stored somewhere else, and cleared, placing the new one in.
    }

    // this is for collecting and organizing a list of values.
    public class StringNodeGroup : INodeGroup<StringNode>
    {
        public StringNodeGroup(string id = null)
        {
            Id = id;
            Nodes = new List<StringNode>();
            AllowFormatting = false;
            TitleMap = "**{0}**";
            ContentMap = "```autohotkey\n{0}```";
            ValueMap = ":: {0}";
            ValueSeparator = "\n";
            UseValueIndex = false; // if the indexing system keeps the initial index number it had
        }

        // page value limits cannot be larger than value limits.
        // if this happens, throw an exception

        // rework constructor and update functions
        public StringNodeGroup(NodeGroupProperties config) : this()
        {
            Update(config: config);
        }

        public string Id { get; } // an optional identifier
        public bool HasId => !string.IsNullOrWhiteSpace(Id);
        public ulong? ReceiverId { get; set; }
        public bool AllowFormatting { get; set; }
        public bool AllowMapping { get; set; }
        public bool UseValueIndex { get; }
        public int? ValueLimit { get; } // prevents the list from going over this.
        public int? PageValueLimit { get; set; }

        public int? Page { get; set; }
        public int? Index { get; set; }
        public string PropertyMap { get; set; }
        public string TitleMap { get; set; }
        public string ContentMap { get; set; }
        public string ValueMap { get; set; }
        public string ValueSeparator { get; set; }
        public List<StringNode> Nodes { get; private set; }

        [MapProperty]
        public string Title { get; set; }

        [MapProperty]
        public string Content
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                List<string> nodeValues = new List<string>();

                // if there's a page limit and is indexed by a page value,
                // skip using (PageValueLimit.Value * Page.Value) so that it functions like a book

                // if there's not a page limit, but is indexed by a page value
                // use the default PageValueLimit specified.

                // if there's a page limit, but is not indexed by a page value, use a scrolling method
                if (PageValueLimit.HasValue)
                {
                    List<StringNode> nodes = Nodes;
                    if (nodes.Count > PageValueLimit.Value)
                        nodes = nodes.Skip(Nodes.Count - PageValueLimit.Value).ToList();
                    for (int i = 0; i < PageValueLimit.Value; i++)
                    {
                        string _content = (i + 1) > nodes.Count ? "" : (!string.IsNullOrWhiteSpace(ValueMap) ?
                            string.Format(ValueMap, nodes[i].ToString() ?? "::") : nodes[i].ToString());
                        nodeValues.Add(_content);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(ValueMap))
                        if (!ValueMap.Contains("{0}"))
                            ValueMap = null;

                    foreach (StringNode node in Nodes)
                        nodeValues.Add(!string.IsNullOrWhiteSpace(ValueMap) ? string.Format(ValueMap, node.ToString()) : node.ToString());
                }

                sb.Append(string.Join(ValueSeparator, nodeValues));

                if (!string.IsNullOrWhiteSpace(ContentMap))
                    if (!ContentMap.Contains("{0}"))
                        ContentMap = null;

                return !string.IsNullOrWhiteSpace(ContentMap) ? string.Format(ContentMap, sb.ToString()) : sb.ToString();
            }
        }

        public bool Update(string title = null, NodeGroupProperties config = null)
        {
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;
            if (config == null)
                return true;
            if (config.AllowFormatting.HasValue)
                AllowFormatting = config.AllowFormatting.Value;
            if (config.Index.HasValue)
                Index = config.Index.Value;
            if (config.Page.HasValue)
                Index = config.Page.Value;
            if (config.ReceiverId.HasValue)
                ReceiverId = config.ReceiverId.Value;
            if (config.PageValueLimit.HasValue)
                PageValueLimit = config.PageValueLimit.Value;

            return true;
        }

        public int AddNode(StringNode node, int? nodeIndex = null)
        {
            node.AllowMapping = false;
            node.AllowFormatting = AllowFormatting;
            if (nodeIndex.HasValue)
            {
                if (Nodes.Any(x => x.Index == nodeIndex))
                {
                    int index = Nodes.IndexOf(Nodes.First(x => x.Index == nodeIndex));
                    Nodes[index] = node;
                    return index;
                }
            }
            if (ValueLimit.HasValue)
            {
                if (Nodes.Count == ValueLimit)
                {
                    StringNode pushedNode = Nodes[0]; // this is the node that's pushed out.
                    Nodes = Nodes.ShiftLeft(1);
                    Nodes.Add(node);
                    return Nodes.IndexOf(node);
                }
                    
            }
            Nodes.Add(node);
            return Nodes.IndexOf(node);
        }

        public bool UpdateNode(int index, NodeProperties config)
        {
            try
            {
                if (UseValueIndex)
                {
                    if (Nodes.Any(x => x.Index == index))
                        Nodes.First(x => x.Index == index).Update(config: config);
                }
                else
                    Nodes[index].Update(config: config);

                return true;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("[Debug] -- Node insertion out of range. --");
                return false;
            }
        }

        public void UpdateNode(int index, StringNode node)
        {
            try
            {
                node.AllowMapping = true;
                node.AllowFormatting = AllowFormatting;

                if (UseValueIndex)
                    Nodes[Nodes.IndexOf(Nodes.First(x => x.Index == index))] = node;
                else
                    Nodes[index] = node;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("[Debug] -- Node insertion out of range. --");
            }
        }

        // list indexing
        public bool RemoveNode(int index)
        {
            if (UseValueIndex)
            {
                if (Nodes.Any(x => x.Index == index))
                {
                    Nodes.RemoveAt(Nodes.IndexOf(Nodes.First(x => x.Index == index)));
                    return true;
                }
                return false;
            }
            else
            {
                if (Nodes.Count >= index + 1)
                {
                    Nodes.RemoveAt(index);
                    return true;
                }

                return false;
            }
        }

        private string FallbackPropertyMap
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                StringMap map = StringMap.FromClass(this);
                if (!string.IsNullOrWhiteSpace(Title))
                {
                    if (!string.IsNullOrWhiteSpace(TitleMap))
                        if (!TitleMap.Contains("{0}"))
                            TitleMap = null;

                    sb.AppendLine(!string.IsNullOrWhiteSpace(TitleMap) ? string.Format(TitleMap, map[nameof(Title)]) : map[nameof(Title)]);
                }
                if (!string.IsNullOrWhiteSpace(Content))
                    sb.Append($"{map[nameof(Content)]}");

                return sb.ToString();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            PropertyMap = PropertyMap ?? FallbackPropertyMap;
            sb.Append(StringMap.Format(PropertyMap, this));
            return sb.ToString();
        }
    }
}
