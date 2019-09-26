namespace Orikivo
{
    public class ServerNotifiers
    {
        public bool? Level { get; set; } = true; // the toggle for levelling up.
        public bool? Status {get; set;} = true; // the toggle for status notifiers.
        public bool? Stream {get; set;} = false; // the toggle for stream notifiers.
        public bool? Update {get; set;} = false; // the toggle for update notifiers.
        public bool? Exception { get; set; } = true; // the toggle for command errors.
    }
}