using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents the pure content of a Clipboard.
    /// </summary>
    public class ClipboardSource
    {
        public string Message { get; set; } // 2000 char limit
        //public MiniEmbed Embed { get; set; } // 8000 char limit
        public LetterAttachment Attachment { get; set; } // the optional attachment appended to the clipboard.

        // make a quick way to read it...?
    }

    public class Clipboard2
    {
        public string Id { get; set; } // The id of the clipboard.
        public Author Author { get; set; }
        public ClipboardSource Source { get; set; } // the actual data that makes up the clipboard.
        //public List<ClipboardReport> Reports { get; set; } = new List<ClipboardReport>(); // reports that this clipboard has.
        public DateTime CreationDate { get; set; }
        public DateTime? LastEdited { get; set; }



        public bool SafeGuard { get; set; } // a toggle made by the poster themselves.
        public bool SafeModGuard { get; set; } // a toggle made by moderators. must be contacted to repair.

        private List<ulong> Upvotes { get; set; }
        private List<ulong> Downvotes { get; set; }
        public long VoteScore { get { return Upvotes.Count - Downvotes.Count; } }
        public ulong Views { get; set; } // amount of times it was called.

        public void Upvote(ulong id)
        {
            if (Upvotes.Contains(id))
                return;
            if (Downvotes.Contains(id))
                Downvotes.Remove(id);
            Upvotes.Add(id);
        }

        public void Downvote(ulong id)
        {
            if (Downvotes.Contains(id))
                return;
            if (Upvotes.Contains(id))
                Upvotes.Remove(id);
            Downvotes.Add(id);
        }

        public void Favorite(Account a)
        {
            // append this clipboard to their favorites by id. whenever they call it
        }
    }

    /// <summary>
    /// Defines the basic properties of an object that can be voted for.
    /// </summary>
    public interface IPostable
    {
        ulong Upvotes { get; set; }
        ulong Downvotes { get; set; }
        ulong Favorites { get; set; }
        ulong Views { get; set; }
        List<PostComment> Comments { get; set; }
    }

    /// <summary>
    /// Represents a comment on a post.
    /// </summary>
    public class PostComment : IScorable
    {
        public Author Author { get; set; } // the author of the comment.
        public string Content { get; set; } // the comment written.
        //public List<PostComment> Replies { get; set; } // the replies appended to the parent comment.
        public List<ulong> Upvotes { get; set; } // a list of all user ids that upvoted the comment.
        public List<ulong> Downvotes { get; set; } // a list of all user ids that downvoted the comment.
        // you cannot both upvote and downvote a post.

        public void Upvote(ulong id)
        {
            if (Upvotes.Contains(id))
                return;
            if (Downvotes.Contains(id))
                Downvotes.Remove(id);
            Upvotes.Add(id);
        }

        public void Downvote(ulong id)
        {
            if (Downvotes.Contains(id))
                return;
            if (Upvotes.Contains(id))
                Upvotes.Remove(id);
            Downvotes.Add(id);
        }
    }

    /// <summary>
    /// Defines the basic properties of an object that can be scored.
    /// </summary>
    public interface IScorable
    {
        List<ulong> Upvotes { get; set; }
        List<ulong> Downvotes { get; set; }
        void Upvote(ulong id);
        void Downvote(ulong id);
    }

    // save a post, and it renders into a cleaner display.
}
