using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    public enum ActionTarget
    {
        // this will target themselves.
        Self = 1,
        // this will target the opposite user
        Opponent = 2
    }

    public class CreatureAction
    {
        // the base action type this move 
        public ActionType Type { get; set; }

        // where does the creature position themselves?
        // if above 0, it will be moving towards, otherwise, move away from player

        // if the creature moves further than the husk's sight distance,
        // they can escape
        public float DistanceToMove { get; set; }

        // 
        public Dictionary<ActionTarget, Effect> Effects { get; set; }
    }

    public class EntityAction
    {
        public float Range { get; set; }
        public float Damage { get; set; }
        public ActionTarget Target { get; set; }
        public List<Effect> Effects { get; set; }

        public Func<Husk, Creature, bool> Criterion { get; set; }
    }

}
