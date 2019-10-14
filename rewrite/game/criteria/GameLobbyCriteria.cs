using System;

namespace Orikivo
{
    // defines the requirements to start a game.
    /// <summary>
    /// The criteria used to define when a Game can start.
    /// </summary>
    public class GameLobbyCriteria
    {
        private GameLobbyCriteria() { }
        public static GameLobbyCriteria FromMode(GameMode mode)
        {
            GameLobbyCriteria clientCriteria = new GameLobbyCriteria();
            switch(mode)
            {
                case GameMode.Werewolf:
                    clientCriteria.RequiredUsers = 5;
                    clientCriteria.UserLimit = 15;
                    return clientCriteria;
                default:
                    throw new Exception("An unknown game mode has been specified.");
            }
        }

        public int RequiredUsers { get; internal set; }
        public int UserLimit { get; internal set; }
        public bool Check(int userCount)
        {
            return UserLimit >= userCount && userCount >= RequiredUsers;
        }
    }
}
