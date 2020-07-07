﻿namespace Arcadia
{
    public class GameDetails
    {
        // what is the name of this game?
        public string Name { get; set; }

        // what is this game about?
        public string Summary { get; set; }

        // how many players are needed for this game?
        public int RequiredPlayers { get; set; } = 1;

        // what is the limit of players that can join?
        public int PlayerLimit { get; set; } = 16;

        // is the specified player count valid to start the game?
        public bool CanStart(int playerCount)
            => playerCount >= RequiredPlayers && playerCount <= PlayerLimit;
    }
}