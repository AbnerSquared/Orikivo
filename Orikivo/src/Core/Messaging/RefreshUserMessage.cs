using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Orikivo
{
    public class RefreshUserMessage
    {
        private readonly IUserMessage _message;

        public RefreshUserMessage(IUserMessage message, TimeSpan refreshRate)
        {
            _message = message;
            RefreshRate = refreshRate;
        }

        private TimeSpan RefreshRate { get; }

        private MessageContent Content { get; set; }

        private DateTime LastRefreshed { get; set; }

        private bool Synced { get; set; }

        public async Task UpdateAsync()
        {
            if (!Synced && (DateTime.UtcNow - LastRefreshed) < RefreshRate)
                return;

            await _message.ModifyAsync(Content?.Content, Content?.Embed?.Build(), new RequestOptions
            {
                RetryMode = RetryMode.RetryTimeouts
            });
            Content = _message.Copy();
            LastRefreshed = DateTime.UtcNow;
            Synced = true;
        }

        public async Task ModifyAsync(string text = null, Embed embed = null)
        {
            if (text == null && embed == null)
                return;

            if ((DateTime.UtcNow - LastRefreshed) < RefreshRate)
            {
                Content = new MessageContent(text, Content?.IsTTS ?? false, embed ?? Content?.Embed?.Build());
                Synced = false;
                return;
            }

            // Create a system to merge MessageContent values together, ordered by priority
            await _message.ModifyAsync(Content?.Content, Content?.Embed?.Build(), new RequestOptions
            {
                RetryMode = RetryMode.RetryTimeouts
            });

            Content = _message.Copy();
            LastRefreshed = DateTime.UtcNow;
            Synced = true;
        }

        public async Task DeleteAsync(RequestOptions options = null)
            => await _message.DeleteAsync(options);

    }
}
