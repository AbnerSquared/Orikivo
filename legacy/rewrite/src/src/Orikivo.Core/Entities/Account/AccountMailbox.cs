using System;
using System.Collections.Generic;
using System.Linq;
namespace Orikivo
{
    public enum WordGuardControl
    {
        Inactive = 1, // ignore...
        Careful = 2, // hidden in spoilers.
        Controlled = 4, // first letter cleans.
        Rigid = 8, // full censors
    }
    /*
    public class AccountMailbox
    {
        public AccountMailbox()
        {
            Mail = new List<OldMail>();
        }
        // This is where Messages are held.
        public List<OldMail> Mail { get; set; } = new List<OldMail>();
        public DateTime LastChecked { get; set; }

        public void Empty()
        {
            Mail.RemoveAll(x => !x.Locked); // removes all mail that has been locked.
        }

        public void Toss(params int[] ids)
        {
            foreach(int id in ids)
            {
                Toss(id);
            }
        }

        public void Toss(int id)
        {
            if (!TryGetMail(id, out OldMail m))
            {
                return;
            }
            if (m.Locked)
            {
                return;
            }

            Toss(m);
        }

        public void Toss(OldMail m)
        {
            if (Mail.Contains(m))
            {
                Mail.Remove(m);
            }
        }

        public void Store(OldMail m)
            => Mail.Add(m);

        public bool HasMail()
            => Mail.Count > 0;

        public bool HasMail(int id)
            => Mail.Any(x => Mail.IndexOf(x) == id);

        public List<OldMail> GetAllMail()
        {
            Refresh();
            return Mail;
        }

        public bool TryGetMail(int index, out OldMail m)
        {
            m = null;
            if (!HasMail())
                return false;

            // make auto corrections a possible .InRange(1, Mail.Count)
            try
            {
                m = Mail[index - 1];
            }
            catch(ArgumentOutOfRangeException)
            {
                return false;
            }
                //m.MarkAsRead();
            return true;
        }

        public void Refresh()
            => LastChecked = DateTime.Now;

        public List<OldMail> GetRecentMail()
        {
            List<OldMail> mail = Mail.Where(x => x.Date > LastChecked).OrderBy(x => x.Date).ToList();
            Refresh();
            return mail;
        }
    }
    */
}