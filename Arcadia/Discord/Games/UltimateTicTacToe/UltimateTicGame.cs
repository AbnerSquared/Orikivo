using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orikivo;

namespace Arcadia.Multiplayer.Games
{
    public class UltimateTicGame : GameBase
    {
        public override string Id => "ultimatetic";

        public override GameDetails Details => new GameDetails
        {
            Name = "Ultimate Tic-Tac-Toe",
            Icon = "⭕",
            Summary = "Tic-Tac-Toe made complex. The first to match 3 boards in a row wins.",
            RequiredPlayers = 2,
            PlayerLimit = 2,
            AllowSessionLeave = false,
            AllowSessionJoin = false
        };

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
                    GameProperty.Create("marker", marker)
                }
            };
        }

        /// <inheritdoc />
        public override List<GameProperty> OnBuildProperties()
        {
            return new List<GameProperty>
            {
                GameProperty.Create("board", UltimateTicBoard.GetDefault()),
                // active_player: The currently active player that is taking their turn
                GameProperty.Create("active_player", TicMarker.Cross),
                GameProperty.Create("locked_direction", (TicDirection)0),
                GameProperty.Create("winner", (TicMarker)0)
            };
        }

        /// <inheritdoc />
        public override List<GameAction> OnBuildActions()
        {
            return new List<GameAction>
            {
                new GameAction(UltimateTicVars.UpdateState, UpdateState),
                new GameAction(UltimateTicVars.GetResults, GetResults)
            };
        }

        /// <inheritdoc />
        public override List<DisplayBroadcast> OnBuildBroadcasts(List<PlayerData> players)
        {
            return new List<DisplayBroadcast>
            {
                new DisplayBroadcast(UltimateTicChannel.Main)
                {
                    Content = new DisplayContent
                    {
                        new Component(UltimateTicChannel.Header, 0, new ComponentFormatter("> {0}'s (**{1}**) Turn\n> {2}", true)),
                        new Component(UltimateTicChannel.Board, 1)
                    },
                    Inputs = new List<IInput>
                    {
                        new TextInput("set", OnSetSlot),
                        new TextInput("resign", OnResign)
                    }
                },
                new DisplayBroadcast(UltimateTicChannel.Results)
                {
                    Content = new DisplayContent
                    {
                        new Component(UltimateTicChannel.Content, 0)
                    }
                }
            };
        }

        public static void OnSetSlot(InputContext ctx)
        {
            var marker = ctx.Player.ValueOf<TicMarker>(UltimateTicVars.Marker);

            if (marker != ctx.Session.ValueOf<TicMarker>(UltimateTicVars.ActivePlayer))
                return;

            bool requireBoard = ctx.Session.ValueOf<TicDirection>(UltimateTicVars.LockedDirection) == 0;

            if ((requireBoard && ctx.Input.Args.Count != 2)
                || (!requireBoard && ctx.Input.Args.Count != 1))
                return;

            string arg1 = ctx.Input.Args.ElementAtOrDefault(0);
            string arg2 = ctx.Input.Args.ElementAtOrDefault(1);

            if (!requireBoard)
                arg2 = arg1;

            TicDirection boardDirection = ctx.Session.ValueOf<TicDirection>(UltimateTicVars.LockedDirection);

            bool isDirectionSuccess = Enum.TryParse(arg2, true, out TicDirection direction);

            if (requireBoard)
                Enum.TryParse(arg1, true, out boardDirection);

            if (!isDirectionSuccess || boardDirection == 0)
                return;

            var slate = ctx.Session.ValueOf<UltimateTicBoard>(UltimateTicVars.Board);

            var board = slate.GetBoard(boardDirection);

            // ignore boards that are already complete
            if (!slate.GetOpenBoards().Contains(board))
                return;

            // Ignore markers that were already placed
            if (!board.GetOpenDirections().Contains(direction))
                return;

            ctx.Session.CancelNewestInQueue();
            ctx.Session.ValueOf<UltimateTicBoard>(UltimateTicVars.Board).GetBoard(boardDirection).SetMarker(marker, direction);

            if (!ctx.Session
                .ValueOf<UltimateTicBoard>(UltimateTicVars.Board)
                .GetOpenBoards()
                .Contains(ctx.Session.ValueOf<UltimateTicBoard>(UltimateTicVars.Board)
                .GetBoard(direction)))
            {
                ctx.Session.SetValue(UltimateTicVars.LockedDirection, (TicDirection)0);
            }
            else
            {
                ctx.Session.SetValue(UltimateTicVars.LockedDirection, direction);
            }

            if (ctx.Session.ValueOf<UltimateTicBoard>(UltimateTicVars.Board).CheckWinState(marker))
            {
                ctx.Session.SetValue(UltimateTicVars.Winner, marker);
                ctx.Session.InvokeAction(UltimateTicVars.GetResults, true);
                return;
            }

            ctx.Session.InvokeAction(UltimateTicVars.UpdateState, true);
        }

        public void OnResign(InputContext ctx)
        {
            var marker = ctx.Player.ValueOf<TicMarker>(TicVars.Marker);

            if (marker != ctx.Session.ValueOf<TicMarker>(TicVars.ActivePlayer))
                return;

            ctx.Session.CancelNewestInQueue();
            ctx.Session.SetValue(UltimateTicVars.Winner, Flip(marker));
            ctx.Session.InvokeAction(UltimateTicVars.GetResults);
        }

        private TicMarker Flip(TicMarker marker)
        {
            if (marker == 0)
                throw new Exception("Expected a specified marker");

            if (marker == TicMarker.Circle)
                return TicMarker.Cross;

            return TicMarker.Circle;
        }

        private void UpdateState(GameContext ctx)
        {
            if (!ctx.Session.ValueOf<UltimateTicBoard>(UltimateTicVars.Board)
                .GetOpenBoards().Any())
            {
                ctx.Session.InvokeAction(UltimateTicVars.GetResults);
                return;
            }

            ctx.Session.SetValue(UltimateTicVars.ActivePlayer, Flip(ctx.Session.ValueOf<TicMarker>(UltimateTicVars.ActivePlayer)));

            DisplayContent content = ctx.Server.GetBroadcast(UltimateTicChannel.Main).Content;
            PlayerData player = ctx.Session.Players.First(x => x.ValueOf<TicMarker>(UltimateTicVars.Marker) == ctx.Session.ValueOf<TicMarker>(TicVars.ActivePlayer));

            TicDirection lockedDirection = ctx.Session.ValueOf<TicDirection>(UltimateTicVars.LockedDirection);

            content.GetValue(UltimateTicChannel.Board).Set(ctx.Session.ValueOf<UltimateTicBoard>(UltimateTicVars.Board).Draw());
            content[UltimateTicChannel.Header].Draw(player.Source.User.Username,
                player.ValueOf<TicMarker>(UltimateTicVars.Marker).ToString(),
                lockedDirection == 0 ? "Unlocked (`set <board> <slot>`)" : $"Locked on Board **{(int)lockedDirection}** (`set <slot>`)");
            content[UltimateTicChannel.Board].Draw();

            ctx.Session.QueueAction(TimeSpan.FromMinutes(2), UltimateTicVars.GetResults);
        }

        private void GetResults(GameContext ctx)
        {
            ctx.Server.SetStateFrequency(GameState.Playing, UltimateTicChannel.Results);
            ctx.Session.SpectateFrequency = UltimateTicChannel.Results;

            DisplayContent content = ctx.Server.GetBroadcast(UltimateTicChannel.Results).Content;

            PlayerData winner = GetPlayer(ctx.Session, ctx.Session.ValueOf<TicMarker>(UltimateTicVars.Winner));

            string winnerText = winner == null ? "Stalemate!" : $"**{winner.Source.User.Username}** Wins!";
            content.GetValue(UltimateTicChannel.Content).Set($"> {winnerText}");
            content[UltimateTicChannel.Content].Draw();

            ctx.Session.QueueAction(TimeSpan.FromSeconds(5), "end");
        }

        private PlayerData GetPlayer(GameSession session, TicMarker marker)
        {
            return session.Players.FirstOrDefault(x => x.ValueOf<TicMarker>(TicVars.Marker) == marker);
        }

        /// <inheritdoc />
        public override async Task OnSessionStartAsync(GameServer server, GameSession session)
        {
            server.SetStateFrequency(GameState.Playing, UltimateTicChannel.Main);
            session.SpectateFrequency = UltimateTicChannel.Main;
            // This is where you can handle who goes first
            bool swapFirst = RandomProvider.Instance.Next(1, 3) == 1;

            if (swapFirst)
                session.SetValue(UltimateTicVars.ActivePlayer, Flip(session.ValueOf<TicMarker>(TicVars.ActivePlayer)));

            session.InvokeAction(UltimateTicVars.UpdateState);
        }

        /// <inheritdoc />
        public override GameResult OnGameFinish(GameSession session)
        {
            var result = new GameResult();
            return result;
        }
    }
}
