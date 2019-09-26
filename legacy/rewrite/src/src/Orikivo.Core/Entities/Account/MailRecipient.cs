namespace Orikivo
{
    /// <summary>
    /// Represents a base user and send status.
    /// </summary>
    public class MailRecipient
    {
        public MailRecipient(ulong id)
        {
            Id = id;
            Sendable = true;
        }
        public MailRecipient(Account a)
        {

        }
        public ulong Id { get; set; }
        public bool Sendable { get; set; }
    }
}
