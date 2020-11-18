using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orikivo;

namespace Arcadia.Multiplayer.Games
{
    public class TicGame : GameBase
    {
        public override string Id => "tictactoe";

        public override GameDetails Details => new GameDetails
        {
            Name = "Tic-Tac-Toe",
            Icon = "❌",
            Summary = "A primitive game of 3 in a row.",
            CanSpectate = true,
            AllowSessionJoin = false,
            AllowSessionLeave = false,
            RequireDirectMessages = false,
            PlayerLimit = 2,
            RequiredPlayers = 2
        };

        /// <inheritdoc />
        public override List<PlayerData> OnBuildPlayers(in IEnumerable<Player> players)
        {
            int index = RandomProvider.Instance.Next(1, 3);
            TicMarker primaryMarker = (TicMarker)index;
            TicMarker secondaryMarker = primaryMarker == TicMarker.Circle ? TicMarker.Cross : TicMarker.Circle;

            return new List<PlayerData>
            {
                BuildPlayer(players.ElementAt(0), primaryMarker),
                BuildPlayer(players.ElementAt(1), secondaryMarker)
            };
        }

        private static PlayerData BuildPlayer(Player player, TicMarker marker)
        {
            return new PlayerData
            {
                Source = player,
                Properties = new List<GameProperty>
                {
                    GameProperty.Create(TicVars.Marker, marker)
                }
            };
        }

        /// <inheritdoc />
        public override List<GameProperty> OnBuildProperties()
        {
            return new List<GameProperty>
            {
                // board: The current state of the game board
                GameProperty.Create(TicVars.Board, TicBoard.GetDefault()),

                // active_player: The currently active player that is taking their turn
                GameProperty.Create(TicVars.ActivePlayer, TicMarker.Cross),

                GameProperty.Create(TicVars.Winner, (TicMarker)0)
            };
        }

        /*
            Game Steps:
            1. A random player is selected
            2. The player that goes first plays their piece.
            3. Handle when the piece is played, and create an idle timeout
        */

        /// <inheritdoc />
        public override List<GameAction> OnBuildActions()
        {
            return new List<GameAction>
            {
                new GameAction(TicVars.UpdateState, UpdateState),
                new GameAction(TicVars.GetResults, GetResults)
            };
        }

        public static void OnSetSlot(InputContext ctx)
        {
            var marker = ctx.Player.ValueOf<TicMarker>(TicVars.Marker);

            if (marker != ctx.Session.ValueOf<TicMarker>(TicVars.ActivePlayer))
                return;

            string input = ctx.Input.Args.FirstOrDefault();
            bool isSuccess = Enum.TryParse(input, true, out TicDirection direction);

            if (!isSuccess || direction == 0)
                return;

            TicBoard board = ctx.Session.ValueOf<TicBoard>(TicVars.Board);

            // Ignore markers that were already placed
            if (!board.GetOpenDirections().Contains(direction))
                return;

            ctx.Session.CancelNewestInQueue();

            ctx.Session.ValueOf<TicBoard>(TicVars.Board).SetMarker(marker, direction);

            if (board.CheckWinState(marker))
            {
                ctx.Session.SetValue(TicVars.Winner, marker);
                ctx.Session.InvokeAction(TicVars.GetResults, true);
                return;
            }

            ctx.Session.InvokeAction(TicVars.UpdateState, true);
        }

        public void OnResign(InputContext ctx)
        {
            var marker = ctx.Player.ValueOf<TicMarker>(TicVars.Marker);

            if (marker != ctx.Session.ValueOf<TicMarker>(TicVars.ActivePlayer))
                return;

            ctx.Session.CancelNewestInQueue();
            ctx.Session.SetValue(TicVars.Winner, Flip(marker));
            ctx.Session.InvokeAction(TicVars.GetResults);
        }

        /// <inheritdoc />
        public override List<DisplayBroadcast> OnBuildBroadcasts(List<PlayerData> players)
        {
            return new List<DisplayBroadcast>
            {
                new DisplayBroadcast(TicChannel.Main)
                {
                    Content = new DisplayContent
                    {
                        new Component(TicChannel.Header, 0, new ComponentFormatter("> **Tic-Tac-Toe**\n> {0}'s ({1}) Turn", true)),
                        new Component(TicChannel.Board, 1)
                    },
                    Inputs = new List<IInput>
                    {
                        new TextInput("set", OnSetSlot),
                        new TextInput("resign", OnResign)
                    }
                },
                new DisplayBroadcast(TicChannel.Results)
                {
                    Content = new DisplayContent
                    {
                        new Component(TicChannel.Content, 0)
                    }
                }
            };
        }

        private void UpdateState(GameContext ctx)
        {
            if (!ctx.Session.ValueOf<TicBoard>(TicVars.Board).GetOpenDirections().Any())
            {
                ctx.Session.InvokeAction(TicVars.GetResults);
                return;
            }

            ctx.Session.SetValue(TicVars.ActivePlayer, Flip(ctx.Session.ValueOf<TicMarker>(TicVars.ActivePlayer)));

            DisplayContent content = ctx.Server.GetBroadcast(TicChannel.Main).Content;
            PlayerData player = ctx.Session.Players.First(x => x.ValueOf<TicMarker>(TicVars.Marker) == ctx.Session.ValueOf<TicMarker>(TicVars.ActivePlayer));

            content.GetValue(TicChannel.Board).Set(ctx.Session.ValueOf<TicBoard>(TicVars.Board).Draw());
            content[TicChannel.Header].Draw(player.Source.User.Username, player.ValueOf<TicMarker>(TicVars.Marker).ToString());
            content[TicChannel.Board].Draw();

            ctx.Session.QueueAction(TimeSpan.FromMinutes(2), TicVars.GetResults);
        }

        private PlayerData GetPlayer(GameSession session, TicMarker marker)
        {
            return session.Players.FirstOrDefault(x => x.ValueOf<TicMarker>(TicVars.Marker) == marker);
        }

        private void GetResults(GameContext ctx)
        {
            ctx.Server.SetStateFrequency(GameState.Playing, TicChannel.Results);
            ctx.Session.SpectateFrequency = TicChannel.Results;

            DisplayContent content = ctx.Server.GetBroadcast(TicChannel.Results).Content;

            PlayerData winner = GetPlayer(ctx.Session, ctx.Session.ValueOf<TicMarker>(TicVars.Winner));

            string winnerText = winner == null ? "Stalemate!" : $"**{winner.Source.User.Username}** Wins!";
            content.GetValue(TicChannel.Content).Set($"> {winnerText}");
            content[TicChannel.Content].Draw();

            ctx.Session.QueueAction(TimeSpan.FromSeconds(5), "end");
        }

        private TicMarker Flip(TicMarker marker)
        {
            if (marker == 0)
                throw new Exception("Expected a specified marker");

            if (marker == TicMarker.Circle)
                return TicMarker.Cross;

            return TicMarker.Circle;
        }

        /// <inheritdoc />
        public override async Task OnSessionStartAsync(GameServer server, GameSession session)
        {
            server.SetStateFrequency(GameState.Playing, TicChannel.Main);
            session.SpectateFrequency = TicChannel.Main;
            // This is where you can handle who goes first
            bool swapFirst = RandomProvider.Instance.Next(1, 3) == 1;

            if (swapFirst)
                session.SetValue(TicVars.ActivePlayer, Flip(session.ValueOf<TicMarker>(TicVars.ActivePlayer)));

            session.InvokeAction(TicVars.UpdateState);
        }

        /// <inheritdoc />
        public override GameResult OnGameFinish(GameSession session)
        {
            var result = new GameResult();
            return result;
        }
    }
}
