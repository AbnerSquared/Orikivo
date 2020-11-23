using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orikivo;
using Orikivo.Drawing;
using Orikivo.Framework;

namespace Arcadia.Multiplayer.Games
{
    public sealed class ChessGame : GameBase
    {
        /// <inheritdoc />
        public override string Id => "chess";

        /// <inheritdoc />
        public override GameDetails Details => new GameDetails
        {
            Name = "Chess",
            Icon = "♟️",
            Summary = "A classic game of immense strategy.",
            RequiredPlayers = 2,
            PlayerLimit = 2
        };

        /// <inheritdoc />
        public override List<GameOption> Options => new List<GameOption>
        {
            GameOption.Create(ChessConfig.RotateBoard, "Rotate board on each turn", true),
            GameOption.Create(ChessConfig.StartingPlayer, "Starting player", ChessStartMode.Random),
            GameOption.Create(ChessConfig.AllowEnPassant, "Allow 'En Passant'", false),
            GameOption.Create(ChessConfig.PieceFormat, "Piece Format", ChessIconFormat.Text)
        };

        [Property("move_history")]
        public List<ChessMove> MoveHistory { get; internal set; } = new List<ChessMove>();

        [Property("board")]
        public ChessBoard Board { get; internal set; }

        [Property("state")]
        public ChessState State { get; internal set; } = ChessState.Active;

        /// <inheritdoc />
        public override List<PlayerData> OnBuildPlayers(in IEnumerable<Player> players)
        {
            return players.Select(CreatePlayer).ToList();
        }

        public PlayerData CreatePlayer(Player player)
        {
            return new PlayerData
            {
                Source = player,
                Properties = new List<GameProperty>
                {
                    GameProperty.Create(ChessVars.Color, player.IsHost ? ChessOwner.White : ChessOwner.Black)
                }
            };
        }

        /// <inheritdoc />
        public override List<GameProperty> OnBuildProperties()
        {
            return new List<GameProperty>
            {
                GameProperty.Create(ChessVars.CurrentColor, ChessOwner.White),
                GameProperty.Create(ChessVars.Board, ChessBoard.GetDefault()),
                GameProperty.Create(ChessVars.GameState, ChessState.Active),
                GameProperty.Create<ChessOwner>(ChessVars.Winner)
            };
        }

        /// <inheritdoc />
        public override List<GameAction> OnBuildActions()
        {
            return new List<GameAction>
            {
                new GameAction(ChessVars.SwapCurrentPlayer, SwapCurrentPlayer),
                new GameAction(ChessVars.GetResults, GetResults)
            };
        }

        // Find a way to make broadcast building easier
        /// <inheritdoc />
        public override List<GameBroadcast> OnBuildBroadcasts(List<PlayerData> players)
        {
            return new List<GameBroadcast>
            {
                new GameBroadcast(ChessChannel.Main)
                {
                    Content = new DisplayContent
                    {
                        new Component(ChessChannel.Header, 0, new ComponentFormatter("> Current Player: **{0}** ({1})")),
                        new Component(ChessChannel.Board, 1)
                    },
                    Inputs = new List<IInput>
                    {
                        // move <piece> <position>
                        // move <piece> <origin> <position>
                        // move <origin> <position>
                        new TextInput("move", OnMovePiece),

                        // resign
                        new TextInput("resign", OnResign)
                    }
                },
                new GameBroadcast(ChessChannel.Results)
                {
                    Content = new DisplayContent
                    {
                        new Component(ChessChannel.Content, 0)
                    }
                }
            };
        }

        private ChessOwner Flip(ChessOwner player)
        {
            return player == ChessOwner.Black ? ChessOwner.White : ChessOwner.Black;
        }

        private void SwapCurrentPlayer(GameContext ctx)
        {
            ctx.Session.SetValue(ChessVars.CurrentColor, Flip(ctx.Session.ValueOf<ChessOwner>(ChessVars.CurrentColor)));

            DisplayContent main = ctx.Server.GetBroadcast(ChessChannel.Main).Content;

            PlayerData current = ctx.Session.Players
                .First(x => x.ValueOf<ChessOwner>(ChessVars.Color) == ctx.Session.ValueOf<ChessOwner>(ChessVars.CurrentColor));

            main.GetValue(ChessChannel.Header).Set(current.Source.User.Username);

            main.GetValue(ChessChannel.Board).Set(
                ctx.Session.ValueOf<ChessBoard>(ChessVars.Board).DrawBoard(
                    ctx.Session.GetConfigValue<bool>(ChessConfig.RotateBoard)
                    ? ctx.Session.ValueOf<ChessOwner>(ChessVars.CurrentColor)
                    : ChessOwner.White, ctx.Session.GetConfigValue<ChessIconFormat>(ChessConfig.PieceFormat)));

            main[ChessChannel.Header].Draw(current.ValueOf<ChessOwner>(ChessVars.Color));
            main[ChessChannel.Board].Draw();

            // In 3 minutes, close the game due to timeout.
            ctx.Session.QueueAction(TimeSpan.FromMinutes(3), ChessVars.GetResults);
        }

        private void GetResults(GameContext ctx)
        {
            ctx.Server.SetStateFrequency(GameState.Playing, ChessChannel.Results);
            DisplayContent results = ctx.Server.GetBroadcast(ChessChannel.Results).Content;

            if (ctx.Session.ValueOf<ChessState>(ChessVars.GameState) == ChessState.Active)
                ctx.Session.SetValue(ChessVars.GameState, ChessState.Timeout);

            PlayerData winner = ctx.Session.Players
                .FirstOrDefault(x => x.ValueOf<ChessOwner>(ChessVars.Color) == ctx.Session.ValueOf<ChessOwner>(ChessVars.Winner));

            string resultWinner = winner == null ? "Stalemate" : $"{winner.Source.User.Username} ({winner.ValueOf<ChessOwner>(ChessVars.Color)}) Wins!";

            results.GetValue(ChessChannel.Content).Set($"> **{resultWinner}**\n> The game has ended in: {ctx.Session.ValueOf<ChessState>(ChessVars.GameState)}.");

            results[ChessChannel.Content].Draw();

            ctx.Session.QueueAction(TimeSpan.FromSeconds(15), "end");
        }

        private void OnMovePiece(InputContext ctx)
        {
            ctx.Session.CancelNewestInQueue();
            // Ignore if the other player is NOT the current player
            if (ctx.Player?.ValueOf<ChessOwner>(ChessVars.Color) != ctx.Session.ValueOf<ChessOwner>(ChessVars.CurrentColor))
                return;

            List<string> args = ctx.Input.Args;

            if (args.Count != 2)
                return;

            ChessPiece piece = ParsePiece(ctx, args[0]);

            if (piece == null)
                return;

            Logger.Debug($"Parsed piece: \"{piece.Owner} {piece.Type} {piece.Rank} {piece.File}\"");

            ChessOwner color = ctx.Session.ValueOf<ChessOwner>(ChessVars.CurrentColor);

            if (piece.Owner != color)
                return;

            if (args[1].Length != 2 || !int.TryParse(args[1][1].ToString(), out int y))
                return;

            Coordinate tile = ParseTile(args[1]);

            ChessBoard board = ctx.Session.ValueOf<ChessBoard>(ChessVars.Board);

            List<Coordinate> valid = board.GetMoves(piece);

            if (!valid.Contains(tile))
                return;

            ChessPiece target = board.GetPiece(tile.X, tile.Y);
            if (target != null)
            {
                if (target.Owner == color)
                    return;

                ctx.Session.ValueOf<ChessBoard>(ChessVars.Board).Pieces.Remove(target);
            }

            Logger.Debug($"Moving piece to: {tile.X} {tile.Y}");

            ctx.Session.ValueOf<ChessBoard>(ChessVars.Board).MovePiece(piece, tile);

            if (board.IsCheckmate(Flip(color)))
            {
                ctx.Session.SetValue(ChessVars.GameState, ChessState.Checkmate);
                ctx.Session.SetValue(ChessVars.Winner, color);
                ctx.Session.InvokeAction(ChessVars.GetResults, true);
                return;
            }

            ctx.Session.InvokeAction(ChessVars.SwapCurrentPlayer, true);
        }

        private void OnResign(InputContext ctx)
        {
            ctx.Session.CancelNewestInQueue();
            // Ignore if the other player is NOT the current player
            if (ctx.Player?.ValueOf<ChessOwner>(ChessVars.Color) != ctx.Session.ValueOf<ChessOwner>(ChessVars.CurrentColor))
                return;

            ctx.Session.SetValue(ChessVars.GameState, ChessState.Resign);
            ctx.Session.SetValue(ChessVars.Winner, Flip(ctx.Session.ValueOf<ChessOwner>(ChessVars.CurrentColor)));
            ctx.Session.InvokeAction(ChessVars.GetResults);
        }

        private Coordinate ParseTile(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new Exception("The specified input was null");

            if (input.Length != 2)
                throw new Exception("Expected a 2-character string");

            int column = GetColumn(input[0]);

            return new Coordinate(column, int.Parse(input[1].ToString()) - 1);
        }

        private ChessPiece ParsePiece(InputContext ctx, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var board = ctx.Session.ValueOf<ChessBoard>(ChessVars.Board);

            if (ChessPiece.TryParse(input, out ChessPieceType type))
            {
                List<ChessPiece> pieces = board.GetPieces(ctx.Session.ValueOf<ChessOwner>(ChessVars.CurrentColor));

                if (pieces.Count(x => x.Type == type) > 1)
                {
                    Logger.Debug("Expected a single piece instance to move");
                    return null;
                }

                return pieces.First(x => x.Type == type);
            }

            if (input.Length != 2 || !int.TryParse(input[1].ToString(), out int y))
                return null;

            Coordinate tile = ParseTile(input);

            Logger.Debug($"Looking at {tile.X} {tile.Y}");

            return board.GetPiece(tile.X, tile.Y);
        }

        private int GetColumn(char c)
        {
            c = char.ToLower(c);

            if (c < 'a' || c > 'a' + 7)
                throw new Exception("Given column index is out of range");

            return c - 'a';
        }

        /// <inheritdoc />
        public override async Task StartAsync(GameServer server, GameSession session)
        {
            server.SetStateFrequency(GameState.Playing, ChessChannel.Main);
            session.SpectateFrequency = ChessChannel.Main;

            ChessStartMode startMode = session.GetConfigValue<ChessStartMode>(ChessConfig.StartingPlayer);

            if (startMode == ChessStartMode.Host || (startMode == ChessStartMode.Random && RandomProvider.Instance.Next(1, 3) == 1))
                session.SetValue(ChessVars.CurrentColor, Flip(session.ValueOf<ChessOwner>(ChessVars.CurrentColor)));

            // set current player based on config
            session.InvokeAction(ChessVars.SwapCurrentPlayer);
        }

        /// <inheritdoc />
        public override GameResult OnGameFinish(GameSession session)
        {
            var result = new GameResult();
            return result;
        }
    }

    // PIECE{CAPTURE}{FILE}{RANK}{PROMOTION}{EN_PASSANT}{CHECK/CHECKMATE}
    // Pxd4
    // Pxd4e.p
    // Pxd4+
    // Pxd4#
    // Pxd8Q
}
