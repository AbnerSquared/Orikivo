using Discord;
using Orikivo.Net;
using System.Threading.Tasks;

namespace Orikivo
{
    public class AttachmentData
    {
        public AttachmentData(Attachment attachment, string name)
        {
            Name = name;
            Extension = EnumUtils.GetUrlExtension(attachment.Filename) ?? ExtensionType.Empty;
            Url = attachment.Url;
        }

        public string Name { get; }

        public string Url { get; }
        
        public ExtensionType Extension { get; }

        public async Task<OriWebResult> GetContentAsync()
        {
            if (!string.IsNullOrWhiteSpace(Url))
                using (var client = new OriWebClient())
                    return await client.RequestAsync(Url);

            return null;
        }
    }
}
