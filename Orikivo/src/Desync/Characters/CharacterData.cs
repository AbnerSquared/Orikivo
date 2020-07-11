using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    public class CharacterData
    {
        public CharacterData(Character character)
        {
            Id = character.Id;
        }

        public string Id { get; }

        public Locator Location { get; internal set; }

        public Destination Destination { get; internal set; }

        public KnownFlag Known { get; internal set; }

        public Dictionary<string, float> Affinity { get; set; }

        public ItemHistory ItemHistory { get; internal set; }

        public DateTime? ExitTime { get; internal set; }

        public string RoutineId { get; internal set; }

        public int RoutineIndex { get; internal set; }

        public bool IsDead { get; set; }

        public void UpdateRoutine()
        {
            if (!ExitTime.HasValue || string.IsNullOrWhiteSpace(RoutineId))
                return;

            if ((DateTime.UtcNow - ExitTime.Value).TotalSeconds > 0)
            {
                Routine routine = Engine.GetCharacter(Id).Routine;
                RoutineEntry entry = routine.GetEntry(RoutineId);
                RoutineNode next = entry.GetNext(RoutineIndex);
                RoutineIndex++;

                if (next == null)
                {
                    entry = routine.GetNextEntry(RoutineId);
                    RoutineId = entry.Id;
                    next = entry.GetInitial();
                    RoutineIndex = 0;
                }
            }
        }

        public void UpdateDestination()
        {

        }

        public Vector2 GetPosition()
        {
            Character character = Engine.GetCharacter(Id);

            if (Location == null && Destination == null)
                return character.DefaultLocation.Vector;

            if (Destination == null)
                return Location.Vector;

            return Destination.Path.GetCurrentPosition(DateTime.UtcNow);
        }
    }
}
