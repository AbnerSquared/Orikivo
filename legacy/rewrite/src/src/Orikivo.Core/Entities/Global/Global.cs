using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using Orikivo.Static;
using Orikivo.Storage;
using Orikivo.Systems.Presets;
using Orikivo.Utility;

namespace Orikivo
{
    // For orikivo's global config.
    public enum UpdateType
    {
        Patch = 0,
        Revision = 1,
        Minor = 2,
        Major = 4
    }

    public class OriVersion
    {
        public OriVersion()
        {
            Major = 0;
            Minor = 0;
            Revision = 5;
            Patch = 0;
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int Patch { get; set; }


        public void Update(UpdateType type)
        {
            switch (type)
            {
                case UpdateType.Major:
                    TickMajor();
                    break;
                case UpdateType.Minor:
                    TickMinor();
                    break;
                case UpdateType.Revision:
                    TickRevision();
                    break;
                default:
                    TickPatch();
                    break;
            }
        }

        public void TickPatch()
        {
            Patch += 1;
            if (Patch > 9)
            {
                while (Patch > 9)
                {
                    Patch = Patch - 10;
                    TickRevision();
                }
            }
        }

        public void TickRevision()
        {
            Revision += 1;
            if (Revision > 9)
            {
                while (Revision > 9)
                {
                    Revision = Revision - 10;
                    TickMinor();
                }
            }
        }

        public void TickMinor()
        {
            Minor += 1;
            if (Minor > 9)
            {
                while (Minor > 9)
                {
                    Minor = Minor - 10;
                    TickMajor();
                }
            }
        }
        public void TickMajor()
        {
            Major += 1;
        }

        public override string ToString()
            => $"{Major}.{Minor}.{Revision}{Patch}";
    }

    public class Changelog
    {
        [JsonConstructor]
        public Changelog(string name, string content, UpdateType type, ulong id, OriVersion version, DateTime date)
        {
            Name = name;
            Content = content;
            Type = type;
            Id = id;
            Version = version;
            Date = date;
        }

        public Changelog(OldGlobal g, string updateName, string content, UpdateType type, ulong id)
        {
            g.Version.Update(type);
            Name = updateName;
            Content = content;
            Type = type;
            Id = id;
            Version = g.Version;
            Date = DateTime.Now;
        }

        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("version")]
        public OriVersion Version { get; set; }

        [JsonProperty("type")]
        public UpdateType Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        public EmbedBuilder Generate()
        {
            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();

            string title = $"Orikivo\n    ↳ {Name}";
            string footer = $"Version {Version.ToString()} | ID: {Id}";

            f.WithText(footer);
            e.WithTitle(title);
            e.WithDescription(Content);
            e.WithFooter(f);
            return e;
        }
    }

    public class Report
    {
        [JsonConstructor]
        public Report(OriReportPriorityType type, Author author, string subject, string command, string content, ulong id)
        {
            Id = id;
            Author = author;
            Type = type;
            Command = command;
            Subject = subject;
            Content = content;

        }

        public Report(OrikivoCommandContext ctx, RestUserMessage message)
        {
            List<Embed> embeds = message.Embeds.ToList();

            if (embeds.Count > 0)
            {
                EmbedBuilder e = embeds.FirstOrDefault().ToEmbedBuilder();

                string sbj = "Subject: ";
                string bid = "BugID: ";

                string[] title = e.Title.Split('\n');

                string.Join("\n", title).Debug();


                string[] top = title[0].Split(" | ");
                string.Join(" | ", top).Debug();


                string emoji = top[0];

                string fullname = top[1];
                fullname.Debug("fullname length");

                string username = fullname.Substring(0, fullname.Length - 5);

                Debugger.Write("i passed this 5");
                // force ignore hashtag.
                string discriminator = top[1].Substring(username.Length + 1);

                Debugger.Write("i passed this 6");
                string sid = e.Footer.Text.Substring(bid.Length);

                Debugger.Write("i passed this 7");
                string command = top[2];
                Debugger.Write("i passed this 8");

                Emoji flag = new Emoji(emoji.Unescape());
                SocketUser u = ctx.Client.GetUser(username, discriminator);

                if (!u.Exists())
                {
                    ctx.Channel.SendMessageAsync($"user not found ({username}, {discriminator})");
                    throw new Exception("Invalid User: No User Fits the Statement.");
                }

                string subject = title[1].Substring(sbj.Length).TryUnwrap("**");
                string content = e.Description;
                ulong id = ulong.Parse(sid);

                Id = id;
                Author = new Author(u);
                Type = flag.GetFlagType();
                Command = command;
                Subject = subject;
                Content = content;
                return;
            }

            throw new Exception("Invalid Message: No Embeds in Container.");
        }

        public Report(OldAccount a, OriReportPriorityType type, ulong id, CommandInfo command, string content, string subject = null)
        {
            Type = type;
            Subject = subject ?? type.GetName();
            Author = new Author(a);
            Command = $"{command.Module.Name ?? command.Module.ToString()}.{command.Name}".ToLower();
            Content = content;
            Id = id;
        }

        public Report(OldAccount a, OriReportPriorityType type, ulong id, string command, string content, string subject = null)
        {
            Type = type;
            Subject = subject ?? type.GetName();
            Author = new Author(a);
            Command = command;
            Content = content;
            Id = id;
        }

        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("type")]
        public OriReportPriorityType Type { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
        

        public EmbedBuilder Generate(OldAccount a)
        {
            EmbedBuilder e = new EmbedBuilder();
            Emoji icon = Type.Icon();
            string user = $"{Author.Name}";
            string subject = $"Subject: {Subject.MarkdownBold()}";
            string title = $"{icon.Pack(a)} | {user} | {Command}\n{subject}";
            string footer = $"Case: {Id}";

            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText(footer);

            e.WithColor(EmbedData.GetColor("error"));
            e.WithTitle(title);
            e.WithDescription(Content);
            e.WithFooter(f);

            return e;
        }

        public string ToString(OldAccount a)
            => $"[{$"{Id}".MarkdownBold()}] {Type.Icon().Pack(a)} {Author.Name}";
    }

    public class IssueIndex
    {
        public ulong Id { get; set; }
        public ulong MessageId { get; set; }
    }

    public class Suggestion
    {
        public Author Author { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
    }

    public class OldGlobal
    {
        [JsonIgnore]
        public const string ClientName = "Orikivo";

        [JsonIgnore]
        public const string ClientVersion = "0.0.580-prerelease";

        [JsonIgnore]
        public const string VotingUrl = "https://discordbots.org/bot/433079994164576268/vote";

        public OldGlobal()
        {
            Activity = new CompactActivity();
        }

        public OriVersion Version { get; set; } = new OriVersion();
        public string Avatar { get; set; } // Image Orikivo uses.
        public string Username { get; set; } // The username Orikivo holds.
        public CompactActivity Activity { get; set; } = new CompactActivity(); // The current activity that Orikivo is doing.
        public UserStatus Status { get; set; } // The current status of Orikivo.
        public Locale Locale { get; set; } // The language Orikivo uses.
        public List<Report> Reports { get; set; } = new List<Report>();
        public List<Report> AcceptedReports { get; set; } = new List<Report>();
        public List<Changelog> Changelogs = new List<Changelog>();
        public ulong IssueOutput { get; set; } // where issues are sent.
        public ulong CaseIncrement { get; set; } // the global report case id counter.
        // issues are only offloaded (sent twice into the same channel, if the original cannot be found.)

        public void AddChangelog(Changelog c)
        {
            if (Changelogs.Contains(c))
                return;

            Changelogs.Add(c);
        }


        // in the case of invalid reports
        public void EnsureCaseIncrement()
        {
            CaseIncrement = 1;
            foreach (Report r in Reports.OrderBy(x => x.Id))
            {
                r.Id = CaseIncrement;
                CaseIncrement += 1;
            }
        }

        public Changelog GetRecentChangelog()
            => Changelogs.OrderByDescending(x => x.Date).FirstOrDefault();
        
        public bool TryGetChangelog(ulong id, out Changelog changelog)
        {
            changelog = null;
            if (!Changelogs.Any(x=> x.Id == id))
            {
                return false;
            }

            changelog = Changelogs.Where(x => x.Id == id).FirstOrDefault();
            return true;
        }

        public void LogReport(Report report)
        {
            if (Reports.Any(x => x.Id == report.Id))
            {
                return;
            }

            Reports.Add(report);
            CaseIncrement += 1;
        }

        public void DeleteReport(Report report)
        {
            Reports.Remove(report);
        }

        public async Task NotifyCompleteReportAsync(OldAccount a, OrikivoCommandContext Context, Report report, Changelog changelog)
        {
            ulong reward = 100;
            string b = $"The report that was accepted ({report.Id}) has been built on {changelog.Name} ({changelog.Id}). Thank you for your input!\nYou have been awarded {EmojiIndex.Balance}{reward.ToPlaceValue().MarkdownBold()} for your time. Keep up the good work.";
            CompactMessage msg = new CompactMessage(b);
            OldMail m = new OldMail("Orikivo", $"({report.Id}) has been built!", msg);
            await m.SendAsync(a, Context.Client);
        }

        public async Task NotifyAcceptedReportAsync(OldAccount a, OrikivoCommandContext Context, Report report)
        {
            CompactMessage msg = new CompactMessage($"The report you submitted ({report.Id}) has been accepted!\nYou will be notified upon the completion of your input.\nThank you for your time!");
            OldMail m = new OldMail("Orikivo", "Your report has been accepted!", msg);
            await m.SendAsync(a, Context.Client);
        }

        public async Task NotifyDeclinedReportAsync(OldAccount a, OrikivoCommandContext Context, Report report, string reason)
        {
            CompactMessage msg = new CompactMessage($"The report you submitted ({report.Id}) has been declined. Here's what the director of the motion stated:```{reason ?? "It failed to meet the criteria of an report."}```");
            OldMail m = new OldMail("Orikivo", "Your report has been declined.", msg);
            await m.SendAsync(a, Context.Client);
        }

        public async Task CompleteReport(OldAccount a, OrikivoCommandContext Context, ulong id, ulong changelogId)
        {
            if (!TryGetChangelog(changelogId, out Changelog changelog))
            {
                await Context.Channel.SendMessageAsync("You need to specify a changelog ID to complete a report.");
                return;
            }

            if (TryGetAcceptedReport(id, out Report report))
            {
                AcceptedReports.Remove(report);
                await NotifyCompleteReportAsync(a, Context, report, changelog);
                return;
            }

            await Context.Channel.SendMessageAsync("The id used does not exist in the list of reports.");
        }

        public async Task AcceptReport(OldAccount a, OrikivoCommandContext Context, ulong id)
        {
            if (TryGetReport(id, out Report report))
            {
                DeleteReport(report);
                AcceptedReports.Add(report);
                await NotifyAcceptedReportAsync(a, Context, report);
                return;
            }

            await Context.Channel.SendMessageAsync("The id used does not exist in the list of reports.");
        }

        public async Task DeclineReport(OldAccount a, OrikivoCommandContext Context, ulong id, string reason = null)
        {
            if (TryGetReport(id, out Report r))
            {
                DeleteReport(r);
                await NotifyDeclinedReportAsync(a, Context, r, reason);
                return;
            }

            await Context.Channel.SendMessageAsync("The id used does not exist in the list of reports.");
        }

        public bool TryGetAcceptedReport(ulong id, out Report report)
        {
            report = null;
            if (!AcceptedReports.Any(x => x.Id == id))
            {
                return false;
            }

            report = AcceptedReports.FirstOrDefault(x => x.Id == id);
            return true;
        }

        public bool TryGetReport(ulong id, out Report report)
        {
            report = null;
            if (!Reports.Any(x => x.Id == id))
            {
                return false;
            }

            report = Reports.FirstOrDefault(x => x.Id == id);
            return true;
        }

        public void Save()
            => Manager.Save(this, FileManager.TryGetPath(this));

        public void SetActivity(CompactActivity a)
        {
            Activity = a ?? new CompactActivity();
        }
        public void SetActivity(string name = null, ActivityType type = ActivityType.Watching)
            => SetActivity(new CompactActivity(name, type));

        // Logging data
        // Module data 
    }

    public class CompactActivity
    {   
        public CompactActivity()
        {
            Name = "an empty name";
            Type = ActivityType.Watching;
        }

        public CompactActivity(string name = null, ActivityType type = ActivityType.Watching)
        {
            Name = name ?? "an empty name";
            Type = type;
        }

        public ActivityType Type { get; set; }

        public string Name { get; set; }

        public override string ToString()
            => $"{Type.ToTypeString()} {Name}";
    }
}