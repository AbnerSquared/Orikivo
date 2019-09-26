using Discord;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orikivo
{
    // this is a display of which the root nodes and groups are set, but can never be removed.
    public class ImmutableDisplay
    {
        public ImmutableDisplay(List<StringNode> nodes, List<StringNodeGroup> groups, DisplayConfig config = null)
        {
            Capacity = 0;
            Nodes = nodes;
            Groups = groups;

            /*
             
             Nodes.ForEach(x =>
            {
                int i = 0;
                x.Index = i;
                i += 1;
            });

            */

            foreach (StringNode node in Nodes)
            {
                node.Index = Capacity;
                Capacity += 1;
            }
            foreach (StringNodeGroup group in Groups)
            {
                group.Index = Capacity;
                Capacity += 1;
            }
        }

        public int Capacity { get; }
        // learn how to make a merged class version.
        public string Title { get; private set; }
        public List<StringNode> Nodes { get; }
        public List<StringNodeGroup> Groups { get; }

        public bool Update(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                Title = title;
                return true;
            }
            return false;
        }
        public bool UpdateNode(int index, NodeProperties properties)
        {
            if (!Nodes.Any(x => x.Index == index))
                throw new Exception("There was no node located at that index.");

            return Nodes.First(x => x.Index == index).Update(config: properties);
        }
        public bool UpdateGroup(int index, NodeGroupProperties properties)
        {
            if (!Groups.Any(x => x.Index == index))
                throw new Exception("There was no group located at that index.");

            return Groups.First(x => x.Index == index).Update(config: properties);
        }

        // using the metadata that defines where the group is.
        public NodeMetadata AddAtGroup(int index, StringNode node)
        {
            if (!Groups.Any(x => x.Index == index))
                throw new Exception("There was no group located at that index.");

            int nodeIndex = Groups.First(x => x.Index == index).AddNode(node);
            return new NodeMetadata(NodeType.Value, nodeIndex, index);
        }

        // public nodemetadata InsertAtGroup(int index, StringNode node, int nodeIndex)
        public bool UpdateAtGroup(int index, int nodeIndex, NodeProperties properties)
        {
            if (!Groups.Any(x => x.Index == index))
                throw new Exception("There was no group located at that index.");

            return Groups.First(x => x.Index == index).UpdateNode(nodeIndex, properties);

        }
        public bool RemoveAtGroup(int index, int nodeIndex)
        {
            if (!Groups.Any(x => x.Index == index))
                throw new Exception("There was no group located at that index.");

            return Groups.First(x => x.Index == index).RemoveNode(nodeIndex);
        }

        public string Content
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(Title))
                    sb.AppendLine($"**{Title}**");

                for (int i = 0; i < Capacity; i++)
                {
                    if (Nodes.Any(x => x.Index == i))
                    {
                        sb.AppendLine(Nodes.First(x => x.Index == i).ToString());
                        sb.AppendLine();
                        continue;
                    }
                    if (Groups.Any(x => x.Index == i))
                    {
                        sb.AppendLine(Groups.First(x => x.Index == i).ToString());
                        sb.AppendLine();
                        continue;
                    }
                }

                return sb.ToString();
            }
        }
    }

    /*
     
        I would like the display to:
        - Have a capacity to be defined.
        - Allow Display.Replace(index, Node); // this would return the node that was replaced
        - Allow Display.Restore(Display); // this would set the entire display to be the one specified.
        - Allow Display.Export(); // this would make DisplayMetadata, which would be a cache for the display.



         
         */
    public class Display
    {
        private int _defaultGroupSize;
        private int _syncKeyLength;
        private bool _showSyncKey;

        public Display(DisplayConfig config = null)
        {
            Nodes = new List<StringNode>();
            Groups = new List<StringNodeGroup>();
            config = config ?? new DisplayConfig();

            Title = config.Title;
            _defaultGroupSize = config.DefaultGroupSize;
            _syncKeyLength = config.SyncKeyLength;
            _showSyncKey = config.ShowSyncKey;
            SyncKey = KeyBuilder.Generate(_syncKeyLength);
            //Metadata = ImmutableList<NodeMetadata>.Empty;
        }

        public int Index { get; private set; }
        public string Title { get; private set; }
        public List<StringNode> Nodes { get; private set; }
        public List<StringNodeGroup> Groups { get; private set; }
        public string SyncKey { get; private set; }

        //public ImmutableList<NodeMetadata> Metadata { get; private set; }

        public string Content
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(Title))
                    sb.AppendLine($"**{Title}**");

                for (int i = 0; i < Index; i++)
                {
                    if (Nodes.Any(x => x.Index == i))
                    {
                        sb.AppendLine(Nodes.First(x => x.Index == i).ToString());
                        sb.AppendLine();
                        continue;
                    }
                    if (Groups.Any(x => x.Index == i))
                    {
                        sb.AppendLine(Groups.First(x => x.Index == i).ToString());
                        sb.AppendLine();
                        continue;
                    }
                }

                if (_showSyncKey)
                    sb.Append($"`{SyncKey}`");

                return sb.ToString();
            }
        }

        public void SetTitle(string title)
        {
            Title = title;
            SyncKey = KeyBuilder.Generate(_syncKeyLength);
        }

        // returns the index of the node added.
        public NodeMetadata AddNode(StringNode node)
        {
            node.Index = Index;
            node.AllowMapping = false;
            Nodes.Add(node);

            Index += 1;
            SyncKey = KeyBuilder.Generate(_syncKeyLength);


            return new NodeMetadata(NodeType.Value, Index - 1);
        }

        // returns the index of the group added
        public NodeMetadata AddGroup(StringNodeGroup group)
        {
            group.Update(config: new NodeGroupProperties { Index = Index, AllowFormatting = false });
            Groups.Add(group);

            Index += 1;
            SyncKey = KeyBuilder.Generate(_syncKeyLength);
            return new NodeMetadata(NodeType.Group, Index - 1);
        }

        public void UpdateNode(int index, string content = null, string title = null, NodeProperties config = null)
        {
            if (Nodes.Any(x => x.Index == index))
            {
                Nodes.First(x => x.Index == index).Update(content, title, config);
                SyncKey = KeyBuilder.Generate(_syncKeyLength);
            }
        }

        public void UpdateGroup(int index, string title, NodeGroupProperties config = null)
        {
            if (Groups.Any(x => x.Index == index))
            {
                Groups.First(x => x.Index == index).Update(title, config);
                SyncKey = KeyBuilder.Generate(_syncKeyLength);
            }
        }

        public void Remove(int index)
        {
            if (Index < index)
                return;

            if (Nodes.Any(x => x.Index == index))
                Nodes.Remove(Nodes.First(x => x.Index == index));
            else if (Groups.Any(x => x.Index == index))
                Groups.Remove(Groups.First(x => x.Index == index));
            else
                return;

            foreach (StringNode node in Nodes)
            {
                if (node.Index > index)
                    node.Index -= 1;
            }
            foreach (StringNodeGroup group in Groups)
            {
                if (group.Index > index)
                    group.Index -= 1;
            }
            Index -= 1;
            SyncKey = KeyBuilder.Generate(_syncKeyLength);
        }

        public void Clear()
        {
            Title = null;
            Nodes.Clear();
            Groups.Clear();
            Index = 0;
            SyncKey = KeyBuilder.Generate(_syncKeyLength);
        }

        public NodeMetadata AddAtGroup(int index, StringNode node)
        {
            if (Groups.Any(x => x.Index == index))
            {
                StringNodeGroup group = Groups.First(x => x.Index == index);
                int nodeIndex = Groups.First(x => x.Index == index).AddNode(node);    
                SyncKey = KeyBuilder.Generate(_syncKeyLength);
                return new NodeMetadata(NodeType.Value, nodeIndex, index);
            }
            return null;
        }

        public NodeMetadata AddAtGroup(int index, string content, ulong? receiverId = null)
            => AddAtGroup(index, new StringNode { Content = content, ReceiverId = receiverId });

        public void UpdateAtGroup(int index, int nodeIndex, NodeProperties config)
        {
            if (Groups.Any(x => x.Index == index))
            {
                Groups.First(x => x.Index == index).UpdateNode(nodeIndex, config);
                SyncKey = KeyBuilder.Generate(_syncKeyLength);
            }
        }

        public void UpdateAtGroup(int index, int nodeIndex, StringNode node)
        {
            if (Groups.Any(x => x.Index == index))
            {
                Groups.First(x => x.Index == index).UpdateNode(nodeIndex, node);
                SyncKey = KeyBuilder.Generate(_syncKeyLength);
            }
        }

        public void UpdateAtGroup(int index, int nodeIndex, string content)
            => UpdateAtGroup(index, nodeIndex, new StringNode { Content = content });

        public bool RemoveAtGroup(int index, int nodeIndex)
        {
            if (Groups.Any(x => x.Index == index))
            {
                bool result = Groups.First(x => x.Index == index).RemoveNode(nodeIndex);
                SyncKey = KeyBuilder.Generate(_syncKeyLength);
                return result;
            }

            return false;
        }

        public string ForReceiver(ulong receiverId)
        {
            Display _display = this;
            foreach (StringNode node in _display.Nodes)
            {
                if (node.ReceiverId.HasValue)
                    if (node.ReceiverId != receiverId)
                        _display.Remove(node.Index.Value);
            }
            foreach (StringNodeGroup group in _display.Groups)
            {
                if (group.ReceiverId.HasValue)
                    if (group.ReceiverId != receiverId)
                        _display.Remove(group.Index.Value);
            }
            _display.SyncKey = SyncKey; // prevents desynchronization;
            // this is toggling specific displays that can be hidden across other guilds.
            return _display.Content;
        }
    }
}
