using System;

namespace Orikivo
{
    // defines the requirements to start a game.
    public class GameBootCriteria
    {
        private GameBootCriteria() { }
        public static GameBootCriteria FromMode(GameMode mode)
        {
            GameBootCriteria clientCriteria = new GameBootCriteria();
            switch(mode)
            {
                case GameMode.Werewolf:
                    clientCriteria.RequiredUsers = 5;
                    clientCriteria.UserLimit = 15;
                    break;
                default:
                    throw new Exception("An unknown game mode has been specified.");
            }
            return clientCriteria;
        }
        public int RequiredUsers { get; internal set; }
        public int UserLimit { get; internal set; }
        public bool Check(int userCount)
        {
            return UserLimit >= userCount && userCount >= RequiredUsers;
        }
    }
}
