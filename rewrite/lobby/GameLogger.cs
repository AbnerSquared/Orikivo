using System;
using System.Text;

namespace Orikivo
{
    public class GameLogger
    {
        private StringBuilder _log; // this just stores all events
        public GameLogger()
        {
            _log = new StringBuilder();
        }

        public void Log(string value)
        {
            _log.AppendLine(value);
            Console.WriteLine(value);
        }

        public override string ToString()
            => _log.ToString();
    }

    //.Log($"[{DateTime.UtcNow.ToShortTimeString()}] #{Id}.ReceiverConnected = {receiver.Id}");
    // .Log($"[{DateTime.UtcNow.ToShortTimeString()}] #{Id}.DisplayUpdated");

    // an entry to place into a game log
    public class GameLogEntry
    {

    }
}
