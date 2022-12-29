using Discord;
using Orikivo.Net;
using System.Threading.Tasks;

namespace Orikivo
{
    public class AttachmentData
    {
        public AttachmentData(IAttachment attachment, string name)
        {
            Name = name;
            Extension = EnumUtils.GetUrlExtension(attachment.Filename) ?? ExtensionType.Empty;
            Url = attachment.Url;
        }

        public string Name { get; }

        public string Url { get; }

        public ExtensionType Extension { get; }

        public async Task<WebResult> GetContentAsync()
        {
            if (string.IsNullOrWhiteSpace(Url))
                return null;

            using var client = new WebClient();
            return await client.RequestAsync(Url);
        }
    }
}
