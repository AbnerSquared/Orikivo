using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // make sure that reports can only be made if errors are public
    // used to contain information on a report a command may have
    public class ReportInfo : ReportBodyInfo
    {
        [JsonConstructor]
        internal ReportInfo(string commandId, OriAuthor author, DateTime createdAt,
            DateTime? editedAt, string title, string content, string imageUrl, ReportBodyInfo lastInfo, ReportFlag level,
            ReportStatus status, List<ulong> upvoteIds, List<ulong> downvoteIds)
        {
            CommandId = commandId;
            Author = author;
            CreatedAt = createdAt;
            EditedAt = editedAt;
            Title = title;
            Content = content;
            ImageUrl = imageUrl;
            LastInfo = lastInfo;
            Flag = level;
            Status = status;
            UpvoteIds = upvoteIds ?? new List<ulong>();
            DownvoteIds = downvoteIds ?? new List<ulong>();
        }

        public ReportInfo(OverloadDisplayInfo overload, OriUser user, ReportFlag level, ReportBodyInfo reportInfo)
        {
            CommandId = overload.Id;
            Author = new OriAuthor(user);
            CreatedAt = DateTime.UtcNow;
            Title = reportInfo.Title;
            Content = reportInfo.Content;
            ImageUrl = reportInfo.ImageUrl;
            Flag = level;
            Status = OriGlobal.DevId == Author.Id ? ReportStatus.Open : ReportStatus.Pending;
            UpvoteIds = new List<ulong>();
            DownvoteIds = new List<ulong>();
        }

        // in seconds
        public static double CooldownLength = 2700;

        [JsonProperty("command_id")]
        public string CommandId { get; } // figure out a way to format that

        [JsonProperty("author")]
        public OriAuthor Author { get; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        [JsonProperty("edited_at")]
        public DateTime? EditedAt { get; private set; }

        [JsonProperty("last_info")]
        public ReportBodyInfo LastInfo { get; private set; }

        [JsonProperty("flag")]
        public ReportFlag Flag { get; } // if incorrect, should be deleted altogether

        [JsonProperty("status")]
        public ReportStatus Status { get; private set; }

        [JsonProperty("close_reason")]
        public string CloseReason { get; private set; }

        [JsonIgnore]
        public bool IsClosed => Status == ReportStatus.Closed;

        [JsonProperty("upvotes")]
        public List<ulong> UpvoteIds { get; }

        [JsonProperty("downvotes")]
        public List<ulong> DownvoteIds { get; }

        [JsonIgnore]
        public bool CanEdit { get { return EditedAt.HasValue ? (DateTime.UtcNow - EditedAt.Value).TotalSeconds >= CooldownLength : true; } }

        [JsonIgnore]
        public bool HasImage { get { return !string.IsNullOrWhiteSpace(ImageUrl); } }

        [JsonIgnore]
        public int Upvotes { get { return UpvoteIds.Count; } }

        [JsonIgnore]
        public int Downvotes { get { return DownvoteIds.Count; } }

        public void SetStatus(ReportStatus status)
        {
            if (IsClosed)
                return;
            Status = status;
        }

        public void AddVote(ulong userId, bool isUpvote = true)
        {
            if (IsClosed)
                return;

            if (UpvoteIds.Contains(userId))
            {
                if (isUpvote)
                    UpvoteIds.Remove(userId);
                else
                {
                    UpvoteIds.Remove(userId);
                    DownvoteIds.Add(userId);
                }
            }
            else if (DownvoteIds.Contains(userId))
            {
                if (isUpvote)
                {
                    DownvoteIds.Remove(userId);
                    UpvoteIds.Add(userId);
                }
                else
                    DownvoteIds.Remove(userId);
            }

            if (isUpvote)
                UpvoteIds.Add(userId);
            else
                DownvoteIds.Add(userId);
        }

        public void Update(string title = null, string content = null, string imageUrl = null)
        {
            if (IsClosed)
                return;
            bool edited = false;
            ReportBodyInfo old = new ReportBodyInfo(Title, Content, ImageUrl);
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;
            if (!string.IsNullOrWhiteSpace(content))
                Content = content;
            if (!string.IsNullOrWhiteSpace(imageUrl))
                ImageUrl = imageUrl;

            if (edited)
            {
                EditedAt = DateTime.UtcNow;
                LastInfo = old;
            }
        }

        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            if (IsClosed)
            {
                sb.AppendLine($"> This report has been closed.");
                if (!string.IsNullOrWhiteSpace(CloseReason))
                    sb.AppendLine($"> `{CloseReason.Escape("`")}`");
            }
            sb.AppendLine($"`{CommandId}` {GetFlagIcon(Flag)} **{Title}**");
            sb.AppendLine(Content);
            return sb.ToString();
        }

        public string GetFlagIcon(ReportFlag flag)
        {
            switch(flag)
            {
                default:
                    return flag.ToString();
            }
        }
    }
}
