using System;

namespace Orikivo
{
    // merge with GameProperties
    public class GameCriteria
    {
        public static GameCriteria FromGame(GameMode gameType)
        {
            GameCriteria gameCriteria = new GameCriteria();
            switch (gameType)
            {
                case GameMode.Werewolf:
                    gameCriteria.RequiredUsers = 6;
                    gameCriteria.UserLimit = 16;
                    return gameCriteria;
                default:
                    Console.WriteLine("[Debug] -- Unknown game mode. Returning null. --");
                    return null;
            }
        }

        public int RequiredUsers { get; private set; }
        public int UserLimit { get; private set; }

        // public bool VoiceChat { get; private set; }
    }
}
