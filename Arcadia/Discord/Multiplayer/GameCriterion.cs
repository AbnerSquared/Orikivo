﻿using System;

namespace Arcadia.Multiplayer
{
    // this is a ruleset that can be used to quickly determine if certain criteria was met
    // It's possible that this can be scrapped; I don't see much use for it
    public class GameCriterion
    {
        public GameCriterion() {}

        public GameCriterion(string id, Func<GameSession, bool> criterion)
        {
            Id = id;
            Criterion = criterion;
        }

        public string Id { get; internal set; }

        // what is the criterion?
        public Func<GameSession, bool> Criterion { get; set; }

        // what is the action i execute whenever this rule is utilized
        public string ActionId { get; set; }
    }
}
