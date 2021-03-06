﻿using System.Collections.Generic;
using Orikivo.Drawing;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the details of a game.
    /// </summary>
    public class GameDetails
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public Sprite Image { get; set; }
        public string Summary { get; set; }
        public int RequiredPlayers { get; set; } = 1;
        public int PlayerLimit { get; set; } = 16;
        public bool CanSpectate { get; set; }
        public bool RequireDirectMessages { get; set; }
        public bool AllowSessionJoin { get; set; }
        public bool AllowSessionLeave { get; set; }
        public List<string> LoadingTips { get; set; }
        public long RequiredWager { get; set; }
    }
}
