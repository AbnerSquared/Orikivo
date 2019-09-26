using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // root orikivo data container
    public class OriGlobal
    {
        [JsonConstructor]
        internal OriGlobal(string prefix, Dictionary<int, ReportInfo> reports)
        {
            Prefix = prefix;
            Reports = reports ?? new Dictionary<int, ReportInfo>();
            ReportIndex = Reports.Count;
            _lobbies = new List<OriLobbyInvoker>();
        }

        public OriGlobal()
        {
            Prefix = DefaultPrefix;
            _lobbies = new List<OriLobbyInvoker>();
            Reports = new Dictionary<int, ReportInfo>();
            ReportIndex = 0;
        }

        public static ulong DevId = 181605794159001601;
        public static ulong SupportGuildId = 456195057373020160;
        public static string DefaultPrefix = "[";
        public static Range PrefixLimit = new Range(1, 16, true);
        public static string ClientVersion = "0.0.5a_dev";

        private string _prefix;

        [JsonProperty("prefix")]
        public string Prefix
        {
            get { return !string.IsNullOrWhiteSpace(_prefix) ? _prefix : DefaultPrefix; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine("[Debug] -- Prefix value is specified. --");
                    if (PrefixLimit.ContainsValue(value.Length))
                    {
                        Console.WriteLine("[Debug] -- Prefix value overridden. --");
                        _prefix = value;
                    }
                }
                else
                {
                    _prefix = null;
                }
            }
        }

        [JsonProperty("report_index")]
        public int ReportIndex { get; private set; }

        [JsonProperty("reports")]
        public Dictionary<int, ReportInfo> Reports { get; }

        // since all of this is managed by GameManager now
        // this can be deleted 

        [JsonIgnore]
        private List<OriLobbyInvoker> _lobbies;
        [JsonIgnore]
        public List<OriLobbyInvoker> Lobbies => _lobbies.Where(x => !x.Closed).ToList();

        public bool HasUserInLobby(ulong id)
            => Lobbies.Any(x => x.Users.Any(y => y.Id == id));

        public void AddLobby(OriLobbyInvoker lobby)
            => _lobbies.Add(lobby);

        public bool HasReport(int index)
            => Reports.ContainsKey(index);

        public ReportInfo GetReport(int index)
            => Reports.ContainsKey(index) ? Reports[index] : null;

        public void AddReport(OriUser user, OverloadDisplayInfo overload, ReportFlag level, ReportBodyInfo info)
        {
            Reports.Add(ReportIndex, new ReportInfo(overload, user, level, info));
            ReportIndex += 1;
        }

        public bool UpdateReport(int index, ReportStatus status)
        {
            if (HasReport(index))
                if (!Reports[index].IsClosed)
                {
                    Reports[index].SetStatus(status);
                    return true;
                }
            return false;
        }

        public void RemoveLobby(OriLobbyInvoker lobby)
        {
            Lobbies.Remove(lobby);
        }
    }
}
