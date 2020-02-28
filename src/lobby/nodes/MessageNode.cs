using System.Linq;
using System.Text;

namespace Orikivo
{
    public class MessageNode : StringNode
    {
        [MapProperty]
        public override string Content 
        {
            get
            {
                Author = Author ?? "Console";
                Message = Message ?? "null";
                return $"[{Author}]: {Message}";
            }
        }

        public string Author { get; set; }

        private string _message;

        public string Message { get { return AllowFormatting ? _message : _message.Escape("|", "`", "*", "_", "~"); } set { _message = value; } }

        public void Update(string author, string message)
        {
            if (!string.IsNullOrWhiteSpace(author) && !string.IsNullOrWhiteSpace(message))
            {
                Author = author;
                Message = message;
            }
        }

        private string FallbackPropertyMap
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                StringMap map = StringMap.FromClass(this);
                Author = Author ?? "Unknown User";
                Message = Message ?? "null";

                if (!string.IsNullOrWhiteSpace(Title))
                    sb.AppendLine($"**{map[nameof(Title)]}**");
                sb.AppendLine($"```{map[nameof(Content)]}```");

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
