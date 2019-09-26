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
            => _log.AppendLine(value);

        public override string ToString()
            => _log.ToString();
    }
}
