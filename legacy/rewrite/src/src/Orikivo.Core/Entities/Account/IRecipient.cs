using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Defines a base Account as legible to receieve mail.
    /// </summary>
    public interface IRecipient
    {
        ulong Limit { get; }
        DateTime LastChecked { get; set; }
        List<Draft> Drafts { get; }
        List<Letter> Inbox { get; }
        List<ulong> BlockedContacts { get; }
        void Block(ulong u);
        void Unblock(ulong u);
        bool TryBlock(ulong u);
        bool TryUnblock(ulong u);
        bool HasUser(ulong u);
        void Refresh();
    }
}
