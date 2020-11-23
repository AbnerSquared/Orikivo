using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    public abstract class GameBase<TPlayer>
        where TPlayer : PlayerBase
    {
        public abstract string Id { get; }

        public abstract GameDetails Details { get; }

        public virtual List<GameOption> Options { get; protected set; } = new List<GameOption>();

        public virtual IReadOnlyList<TPlayer> Players { get; protected set; }

        public abstract void BuildPlayers(in IEnumerable<Player> players);

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

        public abstract List<GameBroadcast> OnBuildBroadcasts(List<PlayerBase> players);

        public abstract Task StartAsync(GameServer server, GameSession session);

        public abstract GameResult OnGameFinish(GameSession session);

        protected void FinalizeResult(GameResult result, GameSession session)
        {
            var properties = new List<GameProperty>();
            properties.AddRange(session.Properties);
            properties.AddRange(session.Options);

            result.GameId = session.Game.Id;
            result.SessionProperties = properties;
            result.SessionDuration = DateTime.UtcNow - session.StartedAt;
        }

        protected virtual ulong CalculateExp(GameSession session, PlayerBase player) => 0;

        public virtual void OnPlayerRemoved(Player player) { }
    }
}
