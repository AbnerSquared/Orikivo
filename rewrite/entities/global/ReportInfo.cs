using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class ReportInfo : ReportBodyInfo
    {
        // the only time this is used is if the exception is deemed critical
        internal static ReportInfo FromException<T>(int id, T exception, string commandId) where T : Exception
            => new ReportInfo(id, commandId, typeof(T).Name, exception.Message, ReportTag.Exception, ReportTag.Auto);

        // exception constructor
        private ReportInfo(int id, string commandId, string title, string content, params ReportTag[] tags)
        {
            Id = id;
            CommandId = commandId;
            Author = new OriAuthor(OriGlobal.ClientName);
            Title = title;
            Content = content;
            Tags = tags?.ToList() ?? new List<ReportTag>();
        }

        [JsonConstructor]
        internal ReportInfo(int id, string commandId, OriAuthor author, DateTime createdAt,
            DateTime? editedAt, string title, string content, string imageUrl, ReportBodyInfo lastInfo, ReportStatus status,
            List<VoteInfo> votes, params ReportTag[] tags)
        {
            Id = id;
            CommandId = commandId;
            Author = author;
            CreatedAt = createdAt;
            EditedAt = editedAt;
            Title = title;
            Content = content;
            ImageUrl = imageUrl;
            LastInfo = lastInfo;
            Tags = tags?.ToList() ?? new List<ReportTag>();
            Status = status;
            Votes = votes ?? new List<VoteInfo>();
        }

        internal ReportInfo(int id, OverloadDisplayInfo overload, OriUser user, ReportBodyInfo reportInfo, params ReportTag[] tags)
        {
            Id = id;
            CommandId = overload.Id;
            Author = new OriAuthor(user);
            CreatedAt = DateTime.UtcNow;
            Title = reportInfo.Title;
            Content = reportInfo.Content;
            ImageUrl = reportInfo.ImageUrl;
            Tags = tags?.ToList() ?? new List<ReportTag>();
            Status = ReportStatus.Open;
            Votes = new List<VoteInfo>();
        }

        // in seconds
        public static double CooldownLength = 2700;

        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("command_id")]
        public string CommandId { get; } // figure out a way to format that

        [JsonProperty("author")]
        public OriAuthor Author { get; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        [JsonProperty("edited_at")]
        public DateTime? EditedAt { get; private set; }

        [JsonProperty("last")] // possible scrap.
        public ReportBodyInfo LastInfo { get; private set; }

        [JsonProperty("tags")]
        public List<ReportTag> Tags { get; } // now an optional ideal

        [JsonProperty("status")]
        public ReportStatus Status { get; private set; }

        [JsonProperty("close_reason")]
        public string CloseReason { get; private set; }

        [JsonIgnore]
        public bool IsClosed => Status == ReportStatus.Closed;

        [JsonProperty("votes")]
        public List<VoteInfo> Votes { get; }

        [JsonIgnore]
        public bool CanEdit => EditedAt.HasValue ? (DateTime.UtcNow - EditedAt.Value).TotalSeconds >= CooldownLength : true;

        [JsonIgnore]
        public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);

        [JsonIgnore]
        public int Upvotes => Votes.Where(x => x.Vote == VoteType.Upvote).Count();

        [JsonIgnore]
        public int Downvotes => Votes.Where(x => x.Vote == VoteType.Downvote).Count();

        // since it's either open or closed, closing is the only way to change status
        public void Close(string reason = null)
        {
            if (IsClosed)
                throw new Exception("This report is already closed.");
            Status = ReportStatus.Closed;
            if (Checks.NotNull(reason, nameof(reason)))
                CloseReason = reason;
        }

        public void AddVote(ulong userId, VoteType vote = VoteType.Upvote)
        {
            if (IsClosed)
                throw new Exception("This report cannot be updated, as it has been closed.");

            if (!Votes.Any(x => x.UserId == userId))
            {
                Votes.Add(new VoteInfo(userId, vote));
                return;
            }

            VoteInfo info = Votes.First(x => x.UserId == userId);
            if (info.Vote == vote) // handle vote removing separately
                return;

            Votes[Votes.IndexOf(info)].Vote = vote;
        }

        public void RemoveVotes(ulong userId)
            => Votes.RemoveAll(x => x.UserId == userId);

        public void Update(string title = null, string content = null, string imageUrl = null)
        {
            if (IsClosed)
                throw new Exception("This report cannot be updated, as it has been closed.");

            bool edited = false;
            ReportBodyInfo old = new ReportBodyInfo(Title, Content, ImageUrl);
            if (Checks.NotNull(title))
            {
                Title = title;
                edited = true;
            }
            if (Checks.NotNull(content))
            {
                Content = content;
                edited = true;
            }
            if (Checks.NotNull(imageUrl))
            {
                ImageUrl = imageUrl;
                edited = true;
            }

            if (edited)
            {
                EditedAt = DateTime.UtcNow;
                LastInfo = old;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IsClosed)
            {
                sb.AppendLine($"> This report has been closed.");
                if (Checks.NotNull(CloseReason))
                    sb.AppendLine($"> `{CloseReason.Escape("`")}`");
            }
            sb.AppendLine($"**{Title}** #{Id}");
            sb.Append($"{(IsClosed ? "📕 **Closed**" : "📖 **Open**")}");
            if (Tags.Count > 0)
                sb.Append($" • {string.Join(' ', Tags.Select(x => $"**#**{x.ToString()}"))}");
            sb.AppendLine();
            sb.AppendLine(Content);
            sb.AppendLine($"`{Author.Name}` **@** `{(EditedAt ?? CreatedAt).ToString("MM/dd/yyyy hh:mm:sstt")}`");
            return sb.ToString();
        }
    }
}
