﻿using System;

namespace Arcadia.Multiplayer
{
    // this should be a system in which a timer is included
    public class GameAction
    {
        public GameAction() { }

        public GameAction(string id, Action<GameContext> onExecute, bool updateOnExecute = true)
        {
            Id = id;
            OnExecute = onExecute;
            UpdateOnExecute = updateOnExecute;
        }
        // what is the name of this action
        public string Id { get; internal set; }

        // what to do when this is called
        // PlayerSessionData is null and can be optional
        public Action<GameContext> OnExecute { get; set; }

        public bool UpdateOnExecute { get; set; } = true;
    }
}
