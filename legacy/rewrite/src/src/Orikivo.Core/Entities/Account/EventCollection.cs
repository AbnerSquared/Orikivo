using System.Collections.Generic;

namespace Orikivo
{
    public class EventCollection
    {
        private Dictionary<EventType, ulong> Collection { get; set; }

        public void Log(EventType eventType)
        {
            if (!Collection.TryGetValue(eventType, out ulong value))
                Collection.Add(eventType, 1);
            else
                Collection[eventType] += 1;
        }

        public ulong this[EventType eventType]
        {
            get
            {
                if (!Collection.TryGetValue(eventType, out ulong value))
                    value = 0;
                return value;
            }
        }
    }
}
