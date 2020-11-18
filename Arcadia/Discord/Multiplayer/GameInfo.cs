using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcadia.Models;
using Orikivo;
using Orikivo.Framework;

namespace Arcadia.Multiplayer
{
    public class GameInfo : IModel<string>
    {
        public GameInfo(GameBase game)
        {
            BaseType = game.GetType();
            Id = game.Id;
            Details = game.Details;
            Options = game.Options;
        }

        public Type BaseType { get; }

        public string Id { get; }

        public string Name => Details.Name;

        public GameDetails Details { get; }

        public IReadOnlyList<GameOption> Options { get; }

        /// <summary>
        /// Builds the <see cref="GameSession"/> for this <see cref="GameBase"/> on the specified <see cref="GameServer"/>.
        /// </summary>
        public virtual async Task BuildAsync(GameServer server)
        {
            if (!(Activator.CreateInstance(BaseType) is GameBase game))
                throw new Exception("Expected inbound game information to initialize a new game");

            // Initialize the new game session
            var session = new GameSession(server, game);
            server.Session = session;

            // server.Connections.ForEach(x => x.State = GameState.Playing);
            // Set all of the server connections to playing
            foreach (ServerConnection connection in server.Connections)
                connection.State = GameState.Playing;

            // NOTE: This is only for debugging, will be removed later.
            server.Session.Game.ExportProperties().ForEach(x => Logger.Debug($"{x.Id}: {x.Value.ToString()}"));
            await server.Session.Game.OnSessionStartAsync(server, session);
        }
    }
}
