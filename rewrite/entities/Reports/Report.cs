using Newtonsoft.Json;
using Orikivo.Unstable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// An object detailing the properties of a possible issue, as described by a user.
    /// </summary>
    public class Report : ReportBody, IReport
    {
        public const int CooldownLength = 2700;

        /// <summary>
        /// Creates a new report from an exception.
        /// </summary>
        internal static Report FromException<TException>(int id, TException exception, string commandId) where TException : Exception
            => new Report(id, commandId, typeof(TException).Name, exception.Message, ReportTag.Exception, ReportTag.Auto);

        [JsonConstructor]
        internal Report(int id, string commandId, Author author, DateTime createdAt,
            DateTime? editedAt, string title, string content, string imageUrl, ReportBody lastInfo, ReportState status,
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
            State = status;
            Votes = votes ?? new List<VoteInfo>();
        }

        internal Report(int id, OverloadDisplayInfo overload, User user, ReportBody reportInfo, params ReportTag[] tags)
        {
            Id = id;
            CommandId = overload.Id;
            Author = new Author(user);
            CreatedAt = DateTime.UtcNow;
            Title = reportInfo.Title;
            Content = reportInfo.Content;
            ImageUrl = reportInfo.ImageUrl;
            Tags = tags?.ToList() ?? new List<ReportTag>();
            State = ReportState.Open;
            Votes = new List<VoteInfo>();
        }

        private Report(int id, string commandId, string title, string content, params ReportTag[] tags)
        {
            Id = id;
            CommandId = commandId;
            Author = new Author(OriGlobal.ClientName);
            Title = title;
            Content = content;
            Tags = tags?.ToList() ?? new List<ReportTag>();
        }

        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("command_id")]
        public string CommandId { get; }

        [JsonProperty("author")]
        public Author Author { get; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        [JsonProperty("edited_at")]
        public DateTime? EditedAt { get; private set; }

        [JsonProperty("last")] // possible scrap.
        public ReportBody LastInfo { get; private set; }

        [JsonProperty("tags")]
        public List<ReportTag> Tags { get; }

        [JsonProperty("state")]
        public ReportState State { get; private set; }

        /// <summary>
        /// The reason this report was closed, if one was specified.
        /// </summary>
        [JsonProperty("close_reason")]
        public string CloseReason { get; private set; }

        [JsonProperty("votes")]
        public List<VoteInfo> Votes { get; }


        [JsonIgnore]
        public bool IsClosed => State == ReportState.Closed;

        [JsonIgnore]
        public bool CanEdit => EditedAt.HasValue ? (DateTime.UtcNow - EditedAt.Value).TotalSeconds >= CooldownLength : true;

        [JsonIgnore]
        public bool HasImage => Checks.NotNull(ImageUrl);

        [JsonIgnore]
        public int Upvotes => Votes.Where(x => x.Vote == VoteType.Upvote).Count();

        [JsonIgnore]
        public int Downvotes => Votes.Where(x => x.Vote == VoteType.Downvote).Count();

        public void Close(string reason = null)
        {
            if (IsClosed)
                throw new Exception("The report specified has already been closed.");
            State = ReportState.Closed;
            if (Checks.NotNull(reason))
                CloseReason = reason;
        }

        public void AddVote(ulong userId, VoteType vote = VoteType.Upvote)
        {
            if (IsClosed)
                throw new Exception("The report specified has already been closed.");

            if (!Votes.Any(x => x.UserId == userId))
            {
                Votes.Add(new VoteInfo(userId, vote));
                return;
            }

            VoteInfo info = Votes.First(x => x.UserId == userId);
            if (info.Vote == vote)
                return;

            Votes[Votes.IndexOf(info)].Vote = vote;
        }

        public void RemoveVotesFrom(ulong userId)
            => Votes.RemoveAll(x => x.UserId == userId);

        public void Update(string title = null, string content = null, string imageUrl = null)
        {
            // TODO: Edit how the update system is used.
            if (IsClosed)
                throw new Exception("The report specified has already been closed.");

            bool edited = false;
            ReportBody old = new ReportBody(Title, Content, ImageUrl);
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

        // TODO: Separate into a formatting service.
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
