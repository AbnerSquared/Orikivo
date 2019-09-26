using System;
using Discord;
using Discord.WebSocket;
using Orikivo.Static;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Orikivo
{/*
    public class OldMail
    {
        public OldMail()
        {
            Date = DateTime.Now;
        }

        public OldMail(string subject, CompactMessage message)
        {
            Author = new Author();
            Date = DateTime.Now;
            Subject = subject;
            Message = message;
        }

        public OldMail(string sender, string subject, CompactMessage message)
        {
            Author = new Author(sender);
            Date = DateTime.Now;
            Subject = subject;
            Message = message;
        }

        public OldMail(OldAccount a, string subject, CompactMessage message)
        {
            Author = new Author(a);
            Date = DateTime.Now;
            Subject = subject;
            Message = message;
        }

        public async Task SendAsync(OldAccount a, DiscordSocketClient client)
        {
            EnsureDate();
            Subject = Subject.TrimEnd('\n');
            a.Mailbox.Store(this);
            await a.NotifyAsync(this, client);
        }

        public void MarkAsRead()
            => Read = true;

        public string GetState(OldAccount a)
        {
            if (Read && !Locked)
                return "";

            return $"{ReadString(a)} {LockString(a)}";
        }

        public string ReadString(OldAccount a, string after = "")
            => Read ? "" : $"{EmojiIndex.Unread.Pack(a)}{after}";

        public string LockString(OldAccount a, string after = "")
            => Locked ? $"{EmojiIndex.Locked.Pack(a)}{after}" : "";

        public void EnsureDate()
            => Date = DateTime.Now;

        public void WithAuthor(OldAccount a)
            => Author = new Author(a);

        public void WithSubject(string subject)
            => Subject = subject;

        public void ToggleLock()
            => Locked = !Locked;

        public Author Author { get; set; } // The account id that sent it.
        public DateTime Date { get; set; } // The time the message was sent.
        public string Subject { get; set; } // The title of a message.
        public CompactMessage Message { get; set; } // The message sent.
        public bool Read { get; set; } // if the mail was read or not.
        public bool Locked { get; set; } // if the mail was locked or not.
        // This is for the mailbox
    }
    */
}