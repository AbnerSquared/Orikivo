using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{/*
    public class OriReport
    {
        public OriReport(Global global, SocketUser user, CommandData command, OriReportPriorityType priority, string issue, string reason)
        {
            Author = new Author(user);
            Command = command;
            CaseId = global.ReportCounter;
            Issue = issue;
            Reason = reason;
            State = global.IsAuthUser(user) ? OriReportStateType.Accepted : OriReportStateType.Pending;
        }

        public Author Author { get; }
        public DateTime SubmittedAt { get; }
        public DateTime? LastEditedAt { get; private set; }
        public CommandData Command { get; }
        public ulong CaseId { get; }
        public string Issue { get; }
        public string Reason { get; private set; }
        public OriReportStateType State { get; private set; }
        public OriReportPriorityType Priority { get; }
        public bool SetState(Global global, SocketUser user, OriReportStateType state, string cause = "")
        {
            if (State == OriReportStateType.Completed || State == OriReportStateType.Declined)
                return false;
            else if (State == OriReportStateType.Pending)
            {
                if (state == OriReportStateType.Accepted || state == OriReportStateType.Declined)
                {
                    if (global.IsAuthUser(user))
                    {
                        State = state;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (State == OriReportStateType.Accepted)
            {
                if (state == OriReportStateType.Completed)
                {
                    if (Global.DevId == user.Id)
                    {
                        State = state;
                        return true;
                    }
                }
            }
            return false;
        }
        public void Edit(string reason)
            => Reason = reason;

    }
    */

    public enum OriReportStateType
    {
        Declined = 0,
        Pending = 1,
        Accepted = 2,
        Completed = 3
    }

    public enum OriReportPriorityType
    {
        Unknown = 0,
        Visual = 1,
        Speed = 2,
        Warning = 3,
        Critical = 4
    }


}
