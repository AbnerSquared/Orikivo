using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    // TODO: Implement a base player class inheritance structure that can store game data for players with custom classes
    // This is to simplify how game logic is required to be written

    public interface IGameBase
    {
        string Id { get; }

        GameDetails Details { get; }

        IReadOnlyList<GameOption> Options { get; }

        IReadOnlyList<PlayerBase> Players { get; }
    }

    /// <summary>
    /// Represents a generic game structure.
    /// </summary>
    /// <typeparam name="TPlayer"></typeparam>
    public abstract class GameBase<TPlayer> : GameBase
        where TPlayer : PlayerBase
    {
        public abstract List<TPlayer> OnBuildTPlayers(in IEnumerable<Player> players);

        /// <summary>
        /// Represents the data of all players for this <see cref="GameBase"/>.
        /// </summary>
        public new IReadOnlyList<TPlayer> Players { get; set; }
    }


    // TODO: Merge GameBase structure with GameSession
    /// <summary>
    /// Represents a generic structure for a game.
    /// </summary>
    public abstract class GameBase
    {
        /// <summary>
        /// Represents the global identifier for this <see cref="GameBase"/>.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Represents the details of this <see cref="GameBase"/>.
        /// </summary>
        public abstract GameDetails Details { get; }

        /// <summary>
        /// Represents all of the possible configurations that this <see cref="GameBase"/> allows.
        /// </summary>
        public virtual List<GameOption> Options { get; protected set; } = new List<GameOption>();

        /// <summary>
        /// Represents the data of all players for this <see cref="GameBase"/>.
        /// </summary>
        public virtual IReadOnlyList<PlayerBase> Players { get; protected set; }

        /// <summary>
        /// When specified, handles building the required data for every player in a <see cref="GameSession"/>.
        /// </summary>
        public abstract List<PlayerData> OnBuildPlayers(in IEnumerable<Player> players);

        // TODO: Remove requirement of this method
        /// <summary>
        /// When specified, handles collecting all of the required global properties for this <see cref="GameBase"/>.
        /// </summary>
        public abstract List<GameProperty> OnBuildProperties();

        /// <summary>
        /// Returns a collection of all specified game properties for this <see cref="GameBase"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GameProperty> ExportProperties()
        {
            return GetType().GetProperties().Where(x => x.GetCustomAttribute<PropertyAttribute>() != null).Select(x => CreateGameProperty(this, x));
        }

        public IEnumerable<GameAction> BuildActions()
        {
            // in: GameContext, out: void
            // Assure that the parameters specified are of GameContext, which return void
            return GetType().GetMethods().Where(x => x.GetCustomAttribute<ActionAttribute>() != null).Select(x => CreateGameAction(this, x));
        }

        private static GameAction CreateGameAction(object callback, MethodInfo method)
        {
            var actionInfo = method.GetCustomAttribute<ActionAttribute>();

            if (actionInfo == null)
                throw new ArgumentException("Expected the specified MethodInfo to have an attribute of type ActionAttribute");

            // This feels a bit wonky, ngl
            return new GameAction(actionInfo.Id, (Action<GameContext>) method.CreateDelegate(typeof(Action<GameContext>), callback), actionInfo.UpdateOnExecute);
        }

        private static GameProperty CreateGameProperty(object callback, PropertyInfo property)
        {
            var propertyInfo = property.GetCustomAttribute<PropertyAttribute>();

            if (propertyInfo == null)
                throw new ArgumentException("Expected the specified PropertyInfo to have an attribute of type PropertyAttribute");

            return GameProperty.Create(propertyInfo.Id, property.GetValue(callback), true);
        }

        // TODO: Instead of force building all actions, automatically retrieve them by labelled attributes
        /// <summary>
        /// When specified, handling collection all of the required actions for this <see cref="GameBase"/>.
        /// </summary>
        public abstract List<GameAction> OnBuildActions();

        /// <summary>
        /// When specified, handles collecting all of the required criteria for this <see cref="GameBase"/>.
        /// </summary>
        public virtual List<GameCriterion> OnBuildRules(List<PlayerData> players)
            => new List<GameCriterion>();

        /// <summary>
        /// When specified, builds all of the required display channels for this <see cref="GameBase"/> and returns them.
        /// </summary>
        /// <param name="players">Represents the collection of players for this <see cref="GameBase"/>.</param>
        public abstract List<DisplayBroadcast> OnBuildBroadcasts(List<PlayerData> players);

        // NOTE: At this point, the GameSession is already built. All you need to do is set the game up for the GameServer
        /// <summary>
        /// Represents the method used to start a <see cref="GameSession"/>.
        /// </summary>
        public abstract Task OnSessionStartAsync(GameServer server, GameSession session);

        // NOTE: This is what Arcadia retrieves when a GameSession finishes, allowing you to update stats, money, and items.
        // NOTE: THIS CANNOT GIVE YOU ITEMS THAT AREN'T MEANT TO ORIGINATE FROM A GAME
        /// <summary>
        /// Represents the method used to safely end a <see cref="GameSession"/>.
        /// </summary>
        public abstract GameResult OnGameFinish(GameSession session);

        // This method is used to finalize the result information for when a game finishes
        protected void FinalizeResult(GameResult result, GameSession session)
        {
            var properties = new List<GameProperty>();
            properties.AddRange(session.Properties);
            properties.AddRange(session.Options);

            result.GameId = session.Game.Id;
            result.SessionProperties = properties;
            result.SessionDuration = DateTime.UtcNow - session.StartedAt;
        }

        // Determine a player's exp amount based on their results
        protected virtual ulong CalculateExp(GameSession session, PlayerData player)
            => 0;

        // TODO: Implement usage of OnPlayerRemoved in games to handle random disconnects
        public virtual void OnPlayerRemoved(Player player)
        {

        }
    }
}
