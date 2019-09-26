using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using Orikivo.Static;
using Orikivo.Systems.Presets;
using Orikivo.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // make new censor bar
    // WordGuard is now three types
    // block, which blocks words entirely |aka| hello => *****.
    // careful, which blocks the internal structure of a word |aka| hello => h****.
    // cautious, which hides the word in spoiler |aka| hello => ||hello||.
    // off, which ignores the word, and leaves it as is |aka| hello => hello.

    /// <summary>
    /// Contains a collection of possible attachment types for a Orikivo.Letter.
    /// </summary>
    public enum LetterAttachmentType
    {
        Url = 1,
        Text = 2,
        Image = 4,
        File = 8,
        Video = 16,
        Audio = 32
    }

    /// <summary>
    /// Represents an attachable file that can be included onto an Orikivo.Letter.
    /// </summary>
    public class LetterAttachment
    {
        public LetterAttachment(string url)
        {
            url.Debug("Attached as URL.");
            Outsourced = true;
            // in the case that the url is escaped...?
            url.Debug();

            url = url.TryParseGyazo();
            url.Debug();

            Url = url;

            if (Path.HasExtension(url))
            {
                string ext = Path.GetExtension(url);
                Type = GetAttachmentType(ext);
            }
            else
            {
                Type = LetterAttachmentType.Url;
            }
        }

        public LetterAttachment(Attachment file)
        {
            file.Url.Debug("Attached as Attachment.");
            Outsourced = false;
            Url = file.Url;
            if (Path.HasExtension(Url))
            {
                string ext = Path.GetExtension(Url);
                Type = GetAttachmentType(ext);
            }
            else
            {
                Type = LetterAttachmentType.File;
            }
        }

        public LetterAttachmentType Type { get; set; }
        // this refers to this being a simple reference..
        public string Url { get; set; }

        public bool Outsourced { get; }
        public bool IsEmbeddable { get { return Type.EqualsAny(LetterAttachmentType.Image); } }
        public bool IsViewable { get { return Type.EqualsAny(LetterAttachmentType.Video, LetterAttachmentType.Audio); } }
        public bool IsWritten { get { return Type.EqualsAny(LetterAttachmentType.Text); } }

        // th
        public string CodeLanguage { get; set; }
        // if .png, .gif, .jpg, .jpeg, etc. place in embed.
        // if .mp4, .mp3, send as seperate file.
        // if .txt, or programming lang ext, show the first 256 chars, and cut off
        // alongside its file.
        // otherwise, send the file as is.
        // if a URL, make a URl info embed for it.

        private LetterAttachmentType GetAttachmentType(string p)
        {
            if (p.EqualsAny(".png", ".gif", ".jpg", ".jpeg"))
                return LetterAttachmentType.Image;
            if (p.EqualsAny(".mp4"))
                return LetterAttachmentType.Video;
            if (p.EqualsAny(".mp3"))
                return LetterAttachmentType.Audio;
            if (p.EqualsAny(".txt", ".cs", ".js", ".html"))
            {
                // removes .{txt}
                CodeLanguage = p.Substring(1);
                return LetterAttachmentType.Text;
            }
            return LetterAttachmentType.File;
        }

        private LetterAttachmentType GetUrlType(string s)
        {
            if (s.EqualsAny(".png", ".gif", ".jpg", ".jpeg"))
                return LetterAttachmentType.Image;
            if (s.EqualsAny(".mp4"))
                return LetterAttachmentType.Video;
            if (s.EqualsAny(".mp3"))
                return LetterAttachmentType.Audio;

            return LetterAttachmentType.Url;
        }
    }

    
    /// <summary>
    /// Represents a composed description of information that has a collection of letter replies.
    /// </summary>
    public class LetterThread
    {
        public LetterThread()
        {
            Values = new List<LetterSource>();
        }

        public LetterThread(LetterSource source) : this()
        {
            Values.Add(source);
        }

        public List<LetterSource> Values { get; set; }
        public LetterSource Root { get { return HasValues ? Values.OrderBy(x => x.Date).First() : null; } }
        private bool HasValues { get { return Values.Funct(); } }
        public void Store(LetterSource source)
        {
            if (!HasValues)
                Values = new List<LetterSource>();
            Values.Add(source);
        }

    }

    /// <summary>
    /// Defines basic properties for an object that can be called by an identifier.
    /// </summary>
    public interface IIdentity
    {
        ulong Id { get; set; }
    }

    /// <summary>
    /// Represents a composed description of information.
    /// </summary>
    public class Letter
    {
        public Letter(OldAccount a, string subject, string message = null, LetterAttachment attachment = null)
        {
            Id = KeyBuilder.Generate(12);
            Author = new Author(a);
            Subject = subject;
            Message = message;
            Attachment = attachment;
            Date = DateTime.UtcNow;
            Read = false;
            Recipients = new List<MailRecipient>();
        }
        public Letter()
        {
            Id = KeyBuilder.Generate(12); // 12 char keybuilder.
            Recipients = null;
            Author = null;
            Forwarder = null;
            Thread = null;
            Read = false;
            Locked = false;
            Marked = false;
            Attachment = null;
            Date = DateTime.UtcNow;
        }
        public LetterSource Source
        {
            get
            {
                return new LetterSource(this);
            }
        }
        public Author Author { get; set; }
        public Author Forwarder { get; set; } // the forwarder of the message.
        // a list of users it was sent to.
        public List<MailRecipient> Recipients { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public LetterAttachment Attachment { get; set; }
        public bool Read { get; set; } // i saw this already..
        public bool Locked { get; set; } // don't delete
        public bool Marked { get; set; } // possible spam??
        public string Id { get; set; } // compare ID's in a thread...
        public bool IsForwarded { get { return Forwarder.Exists(); } } // if the letter was forwarded...?

        public bool HasAttachment { get { return Attachment.Exists(); } }
        public bool IsThreaded { get { return Thread.Exists(); } }
        public LetterThread Thread { get; set; }

        public void Relock()
            => Locked = !Locked;

        // Send a returning letter to another account.
        public async Task ReplyAsync(Account a, Letter l)
        {
            Letter r = AppendThread(l);
            r.Marked = r.TryMark(a);
            a.Mail.Store(l);
        }

        public void MarkRead()
        {
            Read = true;
        }

        public void AddRecipient(ulong id)
            => AddRecipient(new MailRecipient(id));

        public void RemoveRecipient(ulong id)
            => RemoveRecipient(new MailRecipient(id));

        public void AddRecipient(MailRecipient recipient)
            => Recipients.Add(recipient);

        public void RemoveRecipient(MailRecipient recipient)
            => Recipients.Remove(recipient);

        // make a new letter, threading the original letter in a thread.
        public Letter AppendThread(Letter l)
        {
            Letter m = this;
            if (m.IsThreaded)
            {
                m.Thread.Store(m.Source);
            }
            else
            {
                m.Thread = new LetterThread(m.Source);
            }

            m.Author = l.Author;
            m.Date = l.Date;
            m.Subject = l.Subject;
            m.Message = l.Message;
            m.Attachment = l.Attachment;
            m.Read = false;
            m.Locked = false;
            m.Marked = false;

            return m;
        }

        // Mark a letter as read.
        public void Mark()
        {
            Marked = true;
        }

        public bool HasSubject { get { return !string.IsNullOrWhiteSpace(Subject); } }
        public bool HasMessage { get { return !string.IsNullOrWhiteSpace(Message); } }
        public bool HasAuthorName { get { return Author.Exists() ? !string.IsNullOrWhiteSpace(Author.Name) : false; } }

        private List<string> GetValidWords()
        {
            List<string> valid = new List<string>();

            if (HasSubject)
                valid.Add(Subject);
            if (HasMessage)
                valid.Add(Message);
            if (HasAuthorName)
                valid.Add(Author.Name);

            return valid;
        }

        private bool TryMark(Account a)
        {
            List<string> words = GetValidWords();
            if (a.Options.ContainsAnyBannedWord(words))
            {
                Mark();
                /*if (HasSubject)
                    Subject = a.Options.CensorAllBannedWords(Subject);

                if (HasMessage)
                    Message = a.Options.CensorAllBannedWords(Message);

                if (HasAuthorName)
                    Author.Name = a.Options.CensorAllBannedWords(Author.Name);*/
                return true;
            }

            return false;
        }

        // send an exact copy of this message to another account.
        public void Forward(Account client, Account recipient)
        {
            if (recipient.Mail.HasBlocked(Author.Id))
                return;

            recipient.Mail.Store(this);
        }

        public LetterInterpreter Interpret()
            => new LetterInterpreter(this);
    }

    /// <summary>
    /// Defines the basic properties of an object that can be visualized as pixel art.
    /// </summary>
    public interface IPixelate
    {

    }

    /// <summary>
    /// Represents an incomplete description of information.
    /// </summary>
    public class Draft
    {
        public Author Author { get; set; }
        /// <summary>
        /// A collection of users that will receive the message.
        /// </summary>
        public List<ulong> Recipients { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public LetterAttachment Attachment { get; set; }

        // returns a letter built from a draft.
        public Letter Build()
        {
            return new Letter();
        }

        public void Send()
        {
            // if Recipients.Count == 0 >> you cant send without a recipient...
            // foreach (ulong id in Recipients)
            //{
            // Account a = Data.TryGetAccount(id);
            // if (a.Mail.HasBlocked(Author.Id))
            //  continue;
            // a.Mail.Store(Build());
            //}
        }

        public DraftInterpreter Interpret()
            => new DraftInterpreter(this);
    }

    /// <summary>
    /// Represents a class that defines basic time tracking properties.
    /// </summary>
    public class DateTrack
    {
        public DateTime LastViewed { get; set; }
        public void Refresh()
        {
            LastViewed = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Defines basic properties for an object storage that has a limit.
    /// </summary>
    public interface ICapped
    {
        int Count { get; } // amount of x you have
        int Remainder { get; } // amount left to store x.
        int Capacity { get; set; } // the x limit.
    }

    /*
    /// <summary>
    /// Defines basic properties for an object that contains a default value.
    /// </summary>
    public interface IDefault<T>
    {
        T Default { get; }
    }*/

    /// <summary>
    /// Represents a container for a <see cref="Letter"/>.
    /// </summary>
    public class Mailbox : DateTrack, ICapped
    {
        /// <summary>
        /// Constructs a Mailbox with default properties.
        /// </summary>
        public Mailbox()
        {
            Inbox = new List<Letter>();
            Sent = new List<LetterSource>();
            Drafts = new List<Draft>();
            Reports = new List<LetterReport>();
            ContactBlacklist = new List<ulong>();
            //Capacity = 50;
        }

        public static Mailbox Default = new Mailbox();
        public List<Letter> Inbox { get; set; }
        // possible scrap ?? keep in Global.LetterReports...
        // a collection of threaded letters.
        public IEnumerable<Letter> Threads { get { return Inbox.Where(x => x.IsThreaded); } }
        public List<LetterReport> Reports { get; set; }
        public IEnumerable<Letter> Unread { get { return Inbox.Where(x => !x.Read); } }
        public IEnumerable<Letter> Locked { get { return Inbox.Where(x => !x.Locked); } }
        public IEnumerable<Letter> Marked { get { return Inbox.Where(x => x.Marked); } }
        public List<ulong> ContactBlacklist { get; set; }

        // ignore capacities atm...
        public int Count { get { return Inbox.Count; } }
        public int Remainder { get { return Capacity - Count; } }
        public int Capacity {get; set;}

        public List<LetterSource> Sent { get; set; }
        public List<Draft> Drafts { get; set; }
        
        // attempts to clear an inbox, and returns false if there is nothing it can erase...
        public bool TryClearInbox()
        {
            if (HasMail)
            {
                if (!HasLockedMail)
                    ClearInbox();
                return true;
            }
            return false;
        }

        public bool TryClearThreads()
        {
            if (HasMail)
            {
                if (HasThreads)
                    ClearThreads();
                return true;
            }
            return false;
        }
        
        //clearcut methods - will try to execute, without checking...

        /// <summary>
        /// Remove all <see cref="Letter"/> sources that aren't locked from the inbox.
        /// </summary>
        public void ClearInbox()
            => Inbox.RemoveAll(x => !x.Locked);

        public void ClearThreads()
            => Inbox.RemoveAll(x => x.IsThreaded);

        public void ClearMail()
        {
            ClearInbox();
            ClearThreads();
        }

        public void ClearOwnMail()
        {
            ClearDrafts();
            ClearSent();
        }

        public void ClearAll()
        {
            ClearMail();
            ClearOwnMail();
            ClearContacts();
        }

        // takes all Letters that are not marked as spam.
        public void ClearMarked()
            => Inbox = Inbox.TakeWhile(x => !x.Marked).ToList();

        public void ClearSent()
            => Sent = Default.Sent;

        public void ClearDrafts()
            => Drafts = Default.Drafts;

        public void ClearContacts()
            => ContactBlacklist = Default.ContactBlacklist;

        // include checks to see if the contact is blocked..
        public void BlockContact(ulong id)
            => ContactBlacklist.Add(id);

        public void UnblockContact(ulong id)
            => ContactBlacklist.Remove(id);

        public void Cancel(Draft d)
            => Drafts.Remove(d);

        public void Toss(Letter l)
            => Inbox.Remove(l);

        public void Toss(string id)
            => Inbox.Remove(GetLetter(id));

        public void TossAt(int i)
            => Inbox.RemoveAt(i);

        public void TossDraftAt(int i)
            => Drafts.RemoveAt(i);

        public void TossSentAt(int i)
            => Sent.RemoveAt(i);

        public void TossMarkedAt(int i)
            => Inbox.RemoveAt(Inbox.IndexOf(Marked.ElementAt(i)));

        // make a proper way to get data without having to instantiate or pass the datacontainer...
        // as of now, it seems to be the only way...
        public OldAccount GetBlockedContact(OrikivoCommandContext Context, ulong id)
        {
            if (HasBlocked(id))
            {
                return Context.Data.GetOrAddAccount(Context.Client.GetUser(id));
            }

            return null;
        }

        public Draft GetDraftAt(int i)
            => Drafts.ElementAt(i);

        public Letter GetLetter(string id)
            => Inbox.Where(x => x.Id == id).First();

        public Letter GetLetterAt(int i)
            => Inbox.ElementAt(i);

        public LetterSource GetSentAt(int i)
            => Sent.ElementAt(i);

        public Letter GetThread(string id)
            => Threads.Where(x => x.Id == id).First();

        public Letter GetThreadAt(int i)
            => Threads.ElementAt(i);

        // make a proper flag system...?
        public void Flag(OldAccount writer, Letter l, string reason)
        {
            LetterReport lr = new LetterReport(writer, l, reason);
            Reports.Add(lr);
        }

        public IEnumerable<Letter> FromAuthor(ulong id)
            => Inbox.Where(x => x.Author.Id == id);

        public void Store(Letter m)
            => Inbox.Add(m);

        public void Store(Draft d)
            => Drafts.Add(d);

        // checks
        public bool HasAnyData
        {
            get
            {
                return HasAnyMail || HasDrafts || HasSent;
            }
        }

        public bool HasAnyMail
        {
            get
            {
                return HasMail || HasThreads;
            }
        }

        public bool HasLockedMail { get { return Locked.Funct(); } }
        public bool HasMail { get { return Inbox.Funct(); } }
        public bool HasDrafts { get { return Drafts.Funct(); } }
        public bool HasSent { get { return Sent.Funct(); } }
        public bool HasThreads { get { return Threads.Funct(); } }
        public bool HasBlockedContacts { get { return ContactBlacklist.Funct(); } }
        public bool HasUnread { get { return Unread.Funct(); } }
        public bool HasMarked { get { return Marked.Funct(); } }

        public bool HasDraftAt(int i)
        {
            if (HasDrafts)
                return Drafts.Any(x => Drafts.IndexOf(x) == i);
            return false;
        }

        public bool HasLetterAt(int i)
        {
            if (HasMail)
                return Inbox.Any(x => Inbox.IndexOf(x) == i);
            return false;
        }

        public bool ContainsLetter(string id)
        {
            if (HasMail)
                return Inbox.Any(x => x.Id == id);
            return false;
        }

        public bool ContainsAuthor(ulong id)
        {
            if (HasMail)
                return Inbox.Any(x => x.Author.Id == id);
            return false;
        }

        public bool HasAnyBlocked(params ulong[] ids)
            => ids.Any(x => HasBlocked(x));

        public bool HasBlocked(ulong id)
        {
            if (HasBlockedContacts)
                return ContactBlacklist.Contains(id);
            return false;
        }
    }

    // excludes author
    public class LetterReportSource : IStaticReportSource<LetterSource>
    {
        public LetterReportSource(Letter letter, string reason)
        {
            Source = letter.Source;
            Reason = reason;
            Date = DateTime.UtcNow;
        }

        public LetterSource Source { get; }
        public string Reason { get; }
        public DateTime Date { get; }
    }

    // these reports can only be written once.
    public class LetterReport : LetterReportSource, IStaticReport<LetterSource>
    {
        public LetterReport(OldAccount a, Letter letter, string reason) : base(letter, reason)
        {
            Author = new Author(a);
        }

        public Author Author { get; }
    }

    /// <summary>
    /// Represents an Orikivo.Letter's core data.
    /// </summary>
    public class LetterSource
    {
        public LetterSource(Letter l)
        {
            Author = l.Author;
            Subject = l.Subject;
            Message = l.Message;
            Attachment = l.Attachment;
            Date = l.Date;
        }

        public Author Author { get; }
        public string Subject { get; }
        public string Message { get; }
        public LetterAttachment Attachment { get; }
        public DateTime Date { get; }
    }

    /// <summary>
    /// Defines the basic properties of a report with an unspecified type.
    /// </summary>
    public interface IReport : IReportSource
    {
        Author Author { get; set; }
    }

    public interface IReportSource
    {
        string Reason { get; set; }
    }

    public interface IStaticReportSource
    {
        string Reason { get; }
    }

    public interface IStaticReportSource<T> : IStaticReportSource
    {
        T Source { get; }
    }

    public interface IStaticReport : IStaticReportSource
    {
        Author Author { get; }
    }

    public interface IStaticReport<T> : IStaticReport
    {
        T Source { get; }
    }

    /// <summary>
    /// Defines the basic properties of a report of a specified type.
    /// </summary>
    public interface IReport<T> : IReport
    {
        T Source { get; set; }
    }

    /// <summary>
    /// Defines the basic properties of an interpreter with an unspecified type.
    /// </summary>
    public interface IInterpreter
    {
        string ToString(IconFormat f); // show its content as text.
        string ToMarkdown(string type); // show content as markdown.
        MessageStructure ToMessage(IconFormat f); // show content as Embed.
    }

    /// <summary>
    /// Defines the basic properties of an interpreter with a specified type.
    /// </summary>
    public interface IInterpreter<T> : IInterpreter
    {
        T Source { get; set; }
    }

    /// <summary>
    /// Represents an interpreter for Orikivo.Letter.
    /// </summary>
    public class LetterInterpreter : IInterpreter<Letter>
    {
        public LetterInterpreter(Letter source)
        {
            Source = source;
        }

        public Letter Source { get; set; }
        public string ToString(IconFormat f)
        {
            return "";
        }

        public string ToMarkdown(string type)
        {
            return "".DiscordBlock(type);
        }

        public MessageStructure ToMessage(IconFormat f)
        {
            MessageBuilder mb = new MessageBuilder();
            EmbedBuilder eb = Embedder.DefaultEmbed;

            StringBuilder title = new StringBuilder();
            title.AppendLine(Source.Author.Name.MarkdownBold());

            if (Source.Recipients.Funct())
            {
                if (Source.Recipients.Count > 1)
                {
                    title.AppendLine($"**Recipients**: {string.Join(",", Source.Recipients.Select(x => $"{x.Id}".DiscordLine()))}");
                }
            }

            if (Source.IsThreaded)
            {
                title.AppendLine($"**Thread**: {Source.Thread.Root.Subject}\n**SubThread**: {Source.Subject}");
            }
            else
            {
                title.AppendLine($"**Subject**: {Source.Subject}");
            }
            if (Source.Locked)
            {
                title.Insert(0, EmojiIndex.Locked.Format(AccountOptions.Default.IconFormat) + " ");
            }
            if (Source.Marked)
            {
                eb.WithColor(EmbedData.GetColor("error"));
            }
            else if (!Source.Read)
            {
                eb.WithColor(EmbedData.GetColor("owo"));
            }

            eb.WithTitle(title.ToString());
            if (Source.HasMessage)
                eb.WithDescription(Source.Message);

            eb.WithFooter(Source.Id);
            eb.WithTimestamp(Source.Date);

            if (Source.HasAttachment)
            {
                if (Source.Attachment.IsEmbeddable)
                {
                    eb.WithImageUrl(Source.Attachment.Url);
                    // in the case of the image being unable to load..?
                    string msg = $"Attachment [{Source.Attachment.Type}]".DiscordLine().MarkdownLink(Source.Attachment.Url);
                    eb.WithDescription(eb.Description.Exists() ? $"{eb.Description}\n{msg}" : msg);
                }
                else if (Source.Attachment.IsWritten)
                {
                    OriWebClient client = new OriWebClient();
                    WebStringResponse r = client.RequestStringAsync(Source.Attachment.Url).Result;
                    if (r.IsSuccess)
                    {
                        "i passed".Debug();
                        string msg = r.Data.Clean(); // 6 = ``` ``` , 2 = \n \n

                        int limit = 256 - Source.Attachment.CodeLanguage.Length - 6 - 2;

                        if (msg.Length > limit)
                        {
                            msg = msg.Substring(0, limit);
                        }

                        string link = $"Attachment [{Source.Attachment.Type}]".DiscordLine().MarkdownLink(Source.Attachment.Url);
                        msg = msg.DiscordBlock(Source.Attachment.CodeLanguage);
                        eb.WithDescription(eb.Description.Exists() ? $"{eb.Description}\n{msg}{link}": msg + link);
                    }
                }
                else
                {
                    string msg = $"Attachment [{Source.Attachment.Type}]".DiscordLine().MarkdownLink(Source.Attachment.Url);
                    eb.WithDescription(eb.Description.Exists() ? $"{eb.Description}\n{msg}" : msg);
                }
            }

            mb.WithEmbed(eb);

            return mb.Build();
        }
    }

    public class MessageStructure
    {
        public MessageStructure(MessageBuilder builder)
        {
            if (builder.Text.Exists())
            {
                // auto cut off...
                Text = builder.Text; //TryFormat(builder.Message);
            }

            Embed = builder.EmbedBuilder.Build();
            FilePath = builder.FilePath;
        }

        public bool CanSend { get { return !string.IsNullOrWhiteSpace(Text) || Embed.Exists(); } }

        public const int MessageLimit = 2000;
        public const int EmbedDescriptionLimit = 2048;
        public const int EmbedTitleLimit = 256;
        public const int EmbedTotalLimit = 8000;
        public const int EmbedFieldLimit = 25;

        // make the string possible to send...
        private static string TryFormat(string s)
        {
            if (s.Length > MessageLimit)
            {
                s = s.Substring(0, MessageLimit - 3) + "...";
            }
            return s;
        }

        public string Text { get; }
        public Embed Embed { get; }
        public string FilePath { get; }
        public bool HasAttachment { get { return FilePath.Exists(); } }
    }

    // Make a check for the bot developer, to allow inclusion of files on the host.
    public class MessageBuilder
    {
        public string Text { get; set; }
        public EmbedBuilder EmbedBuilder { get; set; }
        public string FilePath { get; set; }

        public MessageBuilder WithText(string s)
        {
            Text = s;
            return this;
        }
        public MessageBuilder WithEmbed(EmbedBuilder eb)
        {
            EmbedBuilder = eb;
            return this;
        }

        // for when it can't be embedded...
        public MessageBuilder WithFile(string path)
        {
            FilePath = path;
            return this;
        }

        public MessageBuilder WithEmbeddedFile(string path)
        {
            FilePath = path;
            if (EmbedBuilder.Exists())
            {
                if (Path.HasExtension(path))
                {
                    if (Path.GetExtension(path).ToLower().EqualsAny(".png", ".gif", ".jpg", ".jpeg"))
                    {
                        if (!EmbedBuilder.ImageUrl.Exists())
                        {
                            EmbedBuilder.WithLocalImageUrl(path);
                        }
                    }
                }
            }
            return this;
        }

        public MessageStructure Build()
            => new MessageStructure(this);
    }

    public class DraftInterpreter : IInterpreter<Draft>
    {
        public DraftInterpreter(Draft source)
        {
            Source = source;
        }
        public Draft Source { get; set; }
        public string ToString(IconFormat f)
        {
            return "";
        }

        public string ToMarkdown(string type)
        {
            return "".DiscordBlock(type);
        }

        public MessageStructure ToMessage(IconFormat f)
        {
            return new MessageBuilder().Build();
        }
    }

    /// <summary>
    /// Represents a creator of a response object.
    /// </summary>
    public class Author
    {
        public Author() { }

        [JsonConstructor]
        public Author(ulong id, string name)
        {
            Id = id;
            Name = name;
        }

        public Author(string name)
        {
            Name = name;
        }

        public Author(SocketUser u)
        {
            Id = u.Id;
            Name = u.ToString();
        }

        public Author(OldAccount a)
        {
            Id = a.Id;
            Name = a.GetDefaultName();
        }

        public ulong Id { get; set; }
        private string _name;
        public string Name { get { return _name ?? "Unknown Author"; } set { _name = value; } }
    }

    /// <summary>
    /// Represents an interpreter for a generic object.
    /// </summary>
    public class Interpreter : IInterpreter
    {
        public Interpreter() { }
        public Interpreter(Type type, string name, string flavortext = null, string description = null, string value = null, Emoji icon = null, ulong? id = null)
        {
            Type = type;
            Id = id;
            Name = name;
            FlavorText = flavortext;
            Description = description;
            Value = value;
            Icon = icon;
        }

        /// <summary>
        /// The type of the object that was interpreted.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// An optional specified identifier.
        /// </summary>
        public ulong? Id { get; set; }

        /// <summary>
        /// The name of the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A short flavor text describing the object.
        /// </summary>
        public string FlavorText { get; set; }

        /// <summary>
        /// A full length description for the object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The set value of the specified object, if any.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// An optional icon used to convey the base object's purpose.
        /// </summary>
        public Emoji Icon { get; set; }

        /// <summary>
        /// Returns a formatted string, displaying all of the information shown.
        /// </summary>
        public string Read(OldAccount a)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine($"{(Icon.Exists() ? $"{Icon.Pack(a)} " : "")}{Name.MarkdownBold()} {(Id.HasValue ? $"{Id}" : "")}");
            str.AppendLine($"{Type.Name} - {Value}");
            str.AppendLine(FlavorText);
            return str.ToString();
        }

        public string ToString(IconFormat f)
        {
            return "";
        }

        public string ToMarkdown(string type)
        {
            return "";
        }

        public MessageStructure ToMessage(IconFormat f)
        {
            return new MessageBuilder().Build();
        }
    }
}






/*
 Text:
    
    [Icon]
    Test Name (Id)
    Type : Value
    Definition

 Markdown:
    [Id()]
    [Icon()]
    [Type()]
    [Name()]
    [Definition()]
    [Value()]

 */
