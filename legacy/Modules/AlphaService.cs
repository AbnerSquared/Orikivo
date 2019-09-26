using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orikivo.Utility;
using System.IO;

namespace Orikivo.Modules
{
    public static class AlphaService
    {
        // Panel = quick embed data.

        // guild coins
        // Server.Leaderboards
        // Global.Leaderboards

        // All executable command that interact with the mailing system.
        #region Account.Mail

        // reply back to a specified mail.
        // mail can be called by name or id.

        public static async Task LetterReplyAsync(OrikivoCommandContext Context, ulong id)
        {
            // Context.NewAccount.TryGetLetter(id, out Letter l);
        }

        // marks all mail as read
        public static async Task MarkMailAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.TryGetLetter(id, out Letter l);
        }

        // marks a letter as read.
        public static async Task MarkLetterAsync(OrikivoCommandContext Context, ulong id)
        {
            // Context.NewAccount.TryGetLetter(id, out Letter l);
        }

        // flag a letter, and remove it from your mailbox.
        public static async Task ReportLetterAsync(OrikivoCommandContext Context, ulong id)
        {
            // Context.NewAccount.TryGetLetter(id, out Letter l);
            // l.FlagAsync(reason); - deletes the letter from the account, and places it into Global.Mail.Reports;
        }

        // builds and sends a draft to all specified users.
        public static async Task SendDraftAsync(OrikivoCommandContext Context, ulong id)
        {
            // Context.NewAccount.TryGetDraft(id, out Draft d);
        }

        // Builds a draft letter.
        public static async Task BuildDraftAsync(OrikivoCommandContext Context, ulong id)
        {
            // Context.NewAccount.ComposeDraft();
        }

        // build and send letter to a user.
        public static async Task ComposeLetterAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.Send(u, );   
        }

        // pass an already existing letter to another user.
        public static async Task ForwardLetterAsync(OrikivoCommandContext Context, SocketUser u)
        {
            // Context.NewAccount.TryGetLetter(id, out Letter l);
            // l.Forward(u);
        }

        // block a specific user from mail interaction.
        public static async Task BlockContactAsync(OrikivoCommandContext Context, SocketUser u)
        {
            // Context.NewAccount.TryBlockContact(u);
        }

        // unblock a blocked user...
        public static async Task UnblockContactAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.TryUnblockContact(u);
        }

        // lock a letter
        public static async Task LockLetterAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.TryGetLetter(id, out Letter l);
            // l.ToggleLock();
        }

        // throw away a letter.
        public static async Task TossLetterAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.TryGetLetter(id, out Letter l);
            // l.Toss();
        }

        // read a letter.
        public static async Task ReadLetterAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.TryGetLetter(id, out Letter l);
            // l.Read();
        }

        public static async Task ViewDraftAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.TryGetDraft(id, out Draft d);
            // d.Read();
        }

        // empty the mailbox
        public static async Task ClearMailAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.Mail.Clear();
        }

        // check the mailbox.
        public static async Task GetMailAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.Mail.Inbox;
        }

        // gets all mail you haven't read yet.
        public static async Task GetUnreadMailAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.Mail.Unread;
        }

        // get all mail you sent.
        public static async Task GetSentAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.Mail.Sent;
        }

        // get all incomplete drafts you built.
        public static async Task GetDraftsAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.Mail.Drafts;
        }

        // removes the report that was submitted in your favor.
        public static async Task RevokeLetterReportAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.Mail.TryRevokeReport(id);
        }

        // toss a specified draft.
        public static async Task TossDraftAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.TryGetDraft(id, out Draft d);
        }

        public static async Task ViewLetterReportsAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.Mail.Reports;
        }

        // get all blocked contacts, with current state.
        public static async Task GetBlockedContactsAsync(OrikivoCommandContext Context)
        {
            // Context.NewAccount.Mail.ContactBlacklist
        }

        // gets info on the contact.
        public static async Task GetBlockedContactAsync(OrikivoCommandContext Context, SocketUser u)
        {
            // Context.NewAccount.Mail.TryGetBlockedContact(u);
        }

        public static async Task DemoLetterAsync(OrikivoCommandContext Context, string subject, string message, string url = null)
        {
            if (url.Exists())
            {
                if (!url.IsProperUrl())
                {
                    url = null;
                }
                /*
                if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                {
                    url = null;
                }
                */
            }
            OldAccount a = Context.Account ?? Context.Data.GetOrAddAccount(Context.User);
            LetterAttachment la = null;
            if (Context.Message.Attachments.Funct())
            {
                 la = new LetterAttachment(Context.Message.Attachments.First());
            }
            else if (url.Exists())
            {
                la = new LetterAttachment(url);
            }

            Letter test = new Letter(a, subject, message, la);

            //Letter test2 = new Letter(a, "hello2", "what is this", null);
            //test2 = test2.AppendThread(test);

            test.AddRecipient(Context.User.Id);
            test.AddRecipient(Context.Client.CurrentUser.Id);
            test.Relock();

            await Context.Channel.SendSourceAsync(test.Interpret().ToMessage(IconFormat.Portable));
        }

        #endregion
    }

    public static class AlphaHelper
    {
        // make a check
        // to where each item is removed
        // and return a list of bools
        // showing the success of each
        // item.

        /*
         public List<bool> TryRemoveMany(List<T> l, params T[] items)
         {
            List<bool> s = new List<bool>();
            foreach (T i in items)
            {
               s.Add(l.Remove(i));
            }
         }
         */
    }
}
