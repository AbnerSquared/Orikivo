using System;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // a node with only content. (if in groups, use frame is always false and titles are always null.)
    public class StringNode : INode
    {
        public StringNode() // if left empty, use the basic config for this node.
        {
            AllowMapping = false;
            AllowFormatting = true;
        }

        public StringNode(NodeProperties config) // instead of properties, pass NodeConfig to the constructor, with NodeProperties being solely for updates.
        {
            Update(config: config);
        }

        // if specified, limits the display to only show on a specific receiver.
        public ulong? ReceiverId { get; set; }
        // essentially an internal reference to the position of this node outside of itself
        public int? Index { get; set; }
        public string TitleMap { get; }
        public string PropertyMap { get; set; }
        public bool AllowMapping { get; set; } // determines if the title or frame is used.
        public bool AllowFormatting { get; set; } // if discord formatting is allowed

        [MapProperty]
        public string Title { get; set; }

        private string _content;

        [MapProperty]
        public virtual string Content { get { return AllowFormatting ? _content : _content.Escape("|", "`", "*", "_", "~"); } set { _content = value; } }

        public void Append(string content)
            => Content += content;

        public bool Update(string content = null, string title = null, NodeProperties config = null)
        {
            if (!string.IsNullOrWhiteSpace(content))
                Content = content;
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;
            if (config == null)
                return true;
            if (config.AllowFormatting.HasValue)
                AllowFormatting = config.AllowFormatting.Value;
            if (config.Index.HasValue)
                Index = config.Index.Value;
            if (config.AllowMapping.HasValue)
                AllowMapping = config.AllowMapping.Value;
            if (config.ReceiverId.HasValue)
                ReceiverId = config.ReceiverId.Value;

            return true;
        }

        private string FallbackPropertyMap
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                StringMap map = StringMap.FromClass(this);

                if (!string.IsNullOrWhiteSpace(Title))
                    sb.AppendLine($"**{map[nameof(Title)]}**");

                sb.Append($"```{map[nameof(Content)]}```");

                return sb.ToString();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!AllowMapping)
                sb.Append(Content);
            else
            {
                PropertyMap = PropertyMap ?? FallbackPropertyMap;
                sb.Append(StringMap.Format(PropertyMap, this));
            }

            return sb.ToString();
        }
    }
}
