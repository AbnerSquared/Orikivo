using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia.Multiplayer.Games
{
    public class ChessBoard
    {
        public ChessBoard()
        {
            StartedAt = DateTime.UtcNow;
        }

        public static readonly string EmptyTile = "•";
        public static readonly int Min = 0;
        public static readonly int Max = 7;

        public static ChessBoard GetDefault()
        {
            return new ChessBoard
            {
                Pieces = new List<ChessPiece>
                {
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Rook,
                        Rank = 0,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Knight,
                        Rank = 1,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Bishop,
                        Rank = 2,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Queen,
                        Rank = 3,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.King,
                        Rank = 4,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Bishop,
                        Rank = 5,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Knight,
                        Rank = 6,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Rook,
                        Rank = 7,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Pawn,
                        Rank = 0,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Pawn,
                        Rank = 1,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Pawn,
                        Rank = 2,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Pawn,
                        Rank = 3,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Pawn,
                        Rank = 4,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Pawn,
                        Rank = 5,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Pawn,
                        Rank = 6,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Type = ChessPieceType.Pawn,
                        Rank = 7,
                        File = 1
                    },

                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Rook,
                        Rank = 0,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Knight,
                        Rank = 1,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Bishop,
                        Rank = 2,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Queen,
                        Rank = 3,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.King,
                        Rank = 4,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Bishop,
                        Rank = 5,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Knight,
                        Rank = 6,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Rook,
                        Rank = 7,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Pawn,
                        Rank = 0,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Pawn,
                        Rank = 1,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Pawn,
                        Rank = 2,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Pawn,
                        Rank = 3,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Pawn,
                        Rank = 4,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Pawn,
                        Rank = 5,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Pawn,
                        Rank = 6,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Type = ChessPieceType.Pawn,
                        Rank = 7,
                        File = 6
                    },
                }
            };
        }

        private static List<Coordinate> GetAllTiles()
        {
            var tiles = new List<Coordinate>();
            for (int x = Min; x <= Max; x++)
            {
                for (int y = Min; y <= Max; y++)
                {
                    tiles.Add(new Coordinate(x, y));
                }
            }

            return tiles;
        }

        public static ChessBoard GetRandom(int whitePieceCount, int blackPieceCount)
        {
            whitePieceCount = whitePieceCount < 0 ? 0 : whitePieceCount;
            blackPieceCount = blackPieceCount < 0 ? 0 : blackPieceCount;

            if (whitePieceCount + blackPieceCount > 64)
                throw new Exception("Only 64 random pieces can be generated on a chess board");

            List<Coordinate> tiles = GetAllTiles();
            var pieces = new List<ChessPiece>();

            for (int i = 0; i < whitePieceCount + blackPieceCount; i++)
            {
                Coordinate tile = Randomizer.Take(tiles);
                var pieceType = (ChessPieceType)RandomProvider.Instance.Next(1, 7);

                pieces.Add(new ChessPiece
                {
                    Owner = i < whitePieceCount ? ChessOwner.White : ChessOwner.Black,
                    Type = pieceType,
                    File = tile.X,
                    Rank = tile.Y
                });
            }

            Console.WriteLine($"Pieces Added:\n{string.Join("\n", pieces.Select(p => $"- {p.Owner.ToString()} {p.Type.ToString()}"))}");

            return new ChessBoard
            {
                Pieces = pieces
            };
        }

        public static string GetPosition(int x, int y)
        {
            if (x > Max || x < Min || y > Max || y < Min)
                throw new Exception("Position out of bounds");

            return $"{GetRow(x)}{y+1}";
        }

        private static char GetRow(int x)
        {
            int offset = 'a' + x;
            char r = (char)offset;
            return r;
        }

        public string DrawBoard(ChessOwner perspective, ChessIconFormat format = ChessIconFormat.Text)
        {
            var text = new StringBuilder();
            text.AppendLine("```");
            text.AppendLine(DrawBorderLabels(perspective));

            bool isFlipped = perspective == ChessOwner.Black;

            for (int column = isFlipped ? Max : Min; isFlipped ? column >= Min : column <= Max; column += isFlipped ? -1 : 1)
            {
                text.Append(column + 1);

                for (int row = isFlipped ? Max : Min; isFlipped ? row >= Min : row <= Max; row += isFlipped ? -1 : 1)
                {
                    ChessPiece piece = GetPiece(row, column);
                    string tile = piece == null ? EmptyTile
                        : ChessPiece.GetString(piece.Type, piece.Owner, format);
                    text.Append($" {tile}");
                }

                text.AppendLine();
            }

            text.AppendLine("```");
            return text.ToString();
        }

        public string DrawMoves(ChessPiece focus, ChessOwner perspective, ChessIconFormat format = ChessIconFormat.Text)
        {
            List<Coordinate> moves = GetMoves(focus);
            var text = new StringBuilder();
            text.AppendLine("```");
            text.AppendLine(DrawBorderLabels(perspective));

            bool isFlipped = perspective == ChessOwner.Black;

            for (int column = isFlipped ? Max : Min; isFlipped ? column >= Min : column <= Max; column += isFlipped ? -1 : 1)
            {
                text.Append(column + 1);

                for (int row = isFlipped ? Max : Min; isFlipped ? row >= Min : row <= Max; row += isFlipped ? -1 : 1)
                {
                    ChessPiece piece = GetPiece(row, column);
                    string tile = piece == null
                        ? moves.Contains(new Coordinate(row, column)) ? "o"
                        : EmptyTile
                        : moves.Contains(new Coordinate(row, column)) && piece.Rank == row && piece.File == column
                            ? "x"
                            : ChessPiece.GetString(piece.Type, perspective, format);
                    text.Append($" {tile}");
                }

                text.AppendLine();
            }

            text.AppendLine("```");
            return text.ToString();
        }

        private static string DrawBorderLabels(ChessOwner perspective)
        {
            return perspective == ChessOwner.Black
                ? "- H G F E D C B A"
                : "- A B C D E F G H";
        }

        public bool IsInCheck(ChessOwner player)
        {
            if (GetPieces(player).All(x => x.Type != ChessPieceType.King))
                return false;

            ChessPiece king = GetKing(player);

            ChessOwner enemy = player == ChessOwner.Black ? ChessOwner.White : ChessOwner.Black;
            return GetPossibleMoves(enemy).Any(x => x.X == king.Rank && x.Y == king.File);
        }

        public ChessPiece GetKing(ChessOwner player)
        {
            return GetPieces(player).FirstOrDefault(x => x.Type == ChessPieceType.King) ?? throw new Exception("Expected a king piece");
        }

        public bool IsCheckmate(ChessOwner player)
        {
            if (!IsInCheck(player))
                return false;

            ChessPiece king = GetKing(player);

            bool canMoveKing = GetDefaultMoves(king).Count > 0;
            List<ChessPiece> blockers = GetBlockerPieces(king);
            List<ChessPiece> attackers = GetAttackerPieces(king);

            if (blockers.Count > 0)
            {
                attackers.RemoveAll(attacker => GetBlockableTiles(attacker, king).Any(c => blockers.Any(blocker => GetDefaultMoves(blocker).Contains(c))));
            }

            // Checkmate occurs if all attackers cannot be blocked or killed
            return !canMoveKing && attackers.Count > 0;
        }

        public List<ChessPiece> GetAttackerPieces(ChessPiece piece)
        {
            ChessOwner enemy = piece.Owner == ChessOwner.Black ? ChessOwner.White : ChessOwner.Black;
            return GetPieces(enemy).Where(x => GetBaseMoves(x).Any(c => c.X == piece.Rank && c.Y == piece.File)).ToList();
        }

        public List<Coordinate> GetAttackerTiles(ChessPiece piece)
        {
            return GetAttackerPieces(piece).Select(x => new Coordinate(x.Rank, x.File)).ToList();
        }

        public List<ChessPiece> GetBlockerPieces(ChessPiece piece)
        {
            List<Coordinate> blockable = GetBlockableTiles(piece);
            return GetPieces(piece.Owner).Where(x => GetBaseMoves(x).Any(c => blockable.Contains(c))).ToList();
        }

        private List<Coordinate> GetBlockableTiles(ChessPiece piece)
        {
            List<ChessPiece> attackers = GetAttackerPieces(piece);
            return attackers.SelectMany(x => GetBlockableTiles(x, piece)).ToList();
        }

        private List<Coordinate> GetBlockableTiles(ChessPiece attacker, ChessPiece target)
        {
            var tiles = new List<Coordinate>();

            if (attacker.Type.EqualsAny(ChessPieceType.Pawn, ChessPieceType.Knight, ChessPieceType.King))
                return tiles;

            ChessDirection direction = GetAttackDirection(attacker, target);
            Move(ref tiles, direction, attacker.Rank, attacker.File, false);

            return tiles;
        }

        private void Move(ref List<Coordinate> moves, ChessDirection direction, int x, int y, bool includeTarget = true)
        {
            switch (direction)
            {
                case ChessDirection.Northwest:
                    MoveNorthWest(ref moves, x, y, includeTarget);
                    break;

                case ChessDirection.North:
                    MoveNorth(ref moves, x, y, includeTarget);
                    break;

                case ChessDirection.Northeast:
                    MoveNorthEast(ref moves, x, y, includeTarget);
                    break;

                case ChessDirection.East:
                    MoveEast(ref moves, x, y, includeTarget);
                    break;

                case ChessDirection.Southeast:
                    MoveSouthEast(ref moves, x, y, includeTarget);
                    break;

                case ChessDirection.South:
                    MoveSouth(ref moves, x, y, includeTarget);
                    break;

                case ChessDirection.Southwest:
                    MoveSouthWest(ref moves, x, y, includeTarget);
                    break;

                case ChessDirection.West:
                    MoveWest(ref moves, x, y, includeTarget);
                    break;
            }
        }

        private ChessDirection GetAttackDirection(ChessPiece attacker, ChessPiece target)
        {
            if (attacker.Type.EqualsAny(ChessPieceType.King, ChessPieceType.Pawn, ChessPieceType.Knight))
                return 0;

            int xDiff = attacker.Rank - target.Rank;
            int yDiff = attacker.File - target.File;

            return GetDirection(xDiff, yDiff);
        }

        private ChessDirection GetDirection(int x, int y)
        {
            if (x != 0 && y != 0 && Math.Abs(x) - Math.Abs(y) != 0)
                return 0;

            if (x > 0)
            {
                if (y > 0)
                    return ChessDirection.Northwest;

                if (y < 0)
                    return ChessDirection.Southwest;

                return ChessDirection.West;
            }

            if (x < 0)
            {
                if (y > 0)
                    return ChessDirection.Northeast;

                if (y < 0)
                    return ChessDirection.Southeast;

                return ChessDirection.East;
            }

            if (y > 0)
                return ChessDirection.North;

            if (y < 0)
                return ChessDirection.South;

            return 0;
        }

        private List<ChessPiece> GetDefenderPieces(ChessPiece piece)
        {
            List<ChessPiece> attackers = GetAttackerPieces(piece);

            return GetPieces(piece.Owner)
                .Where(x => GetBaseMoves(x)
                    .Any(c => attackers
                        .Any(a => a.Rank == c.X && a.File == c.Y)))
                .ToList();
        }

        public ChessPiece GetPiece(int rank, int file)
            => Pieces.FirstOrDefault(p => p.Rank == rank && p.File == file);

        public List<Coordinate> GetUsedCoordinates()
        {
            return Pieces.Select(x => new Coordinate(x.Rank, x.File)).ToList();
        }

        public List<ChessPiece> GetPieces(ChessOwner owner)
        {
            return Pieces.Where(x => x.Owner == owner).ToList();
        }

        public List<Coordinate> GetUsedTiles(ChessOwner owner)
        {
            return Pieces.Where(x => x.Owner == owner).Select(x => new Coordinate(x.Rank, x.File)).ToList();
        }

        private void AddPawnTiles(ref List<Coordinate> moves, ChessPiece piece)
        {
            bool isFlipped = piece.Owner == ChessOwner.Black;

            if (piece.File < Max && piece.File > Min)
            {
                ChessPiece front = GetPiece(piece.Rank, isFlipped ? piece.File + 1 : piece.File - 1);

                if (front != null)
                    moves.RemoveAll(x => x.X == front.Rank && x.Y == front.File);
            }

            if (piece.Rank < Max)
            {
                ChessPiece left = GetPiece(piece.Rank - 1, isFlipped ? piece.File + 1 : piece.File - 1);

                if (left != null && left.Owner != piece.Owner)
                    moves.Add(new Coordinate(left.Rank, left.File));
            }

            if (piece.Rank > Min)
            {
                ChessPiece right = GetPiece(piece.Rank + 1, isFlipped ? piece.File + 1 : piece.File - 1);

                if (right != null && right.Owner != piece.Owner)
                    moves.Add(new Coordinate(right.Rank, right.File));
            }

            // en passant
            if (LastMove != null && LastMove.Piece == ChessPieceType.Pawn && LastMove.Player != piece.Owner)
            {
                int fromY = (LastMove.Player == ChessOwner.Black) ? 1 : 6;
                int toY = (LastMove.Player == ChessOwner.Black) ? 3 : 4;
                Console.WriteLine($"{LastMove.Piece} ({LastMove.Rank}, {LastMove.File} [{fromY}] => {LastMove.To.X}, {LastMove.To.Y} [{toY}])");

                if (LastMove.File == fromY && LastMove.To.Y == toY)
                {
                    // Left
                    if (piece.Rank - LastMove.Rank == 1)
                    {
                        int x = piece.Rank - 1;
                        int y = isFlipped ? piece.File + 1 : piece.File - 1;

                        moves.Add(new Coordinate(x, y));
                    }
                    else if (piece.Rank - LastMove.Rank == -1)
                    {
                        int x = piece.Rank + 1;
                        int y = isFlipped ? piece.File + 1 : piece.File - 1;

                        moves.Add(new Coordinate(x, y));
                    }
                }
            }
        }

        private List<Coordinate> GetRequiredMoves(ChessPiece piece)
        {
            ChessPiece king = GetKing(piece.Owner);

            List<Coordinate> available = GetBlockableTiles(king);
            List<Coordinate> attackable = GetAttackerTiles(king);
            List<Coordinate> moves = GetDefaultMoves(piece);

            moves.RemoveAll(x => !available.Contains(x) && !attackable.Contains(x));

            Console.WriteLine($"Available required moves for {piece.Owner} {piece.Type}:\n{string.Join("\n", moves)}");
            return moves;
        }

        public List<Coordinate> GetMoves(ChessPiece piece)
        {
            if (IsInCheck(piece.Owner) && piece.Type != ChessPieceType.King)
                return GetRequiredMoves(piece);

            return GetDefaultMoves(piece);
        }

        private List<Coordinate> GetDefaultMoves(ChessPiece piece)
        {
            var moves = GetBaseMoves(piece.Owner, piece.Type, piece.Rank, piece.File)
                .Where(x => !GetUsedTiles(piece.Owner).Contains(x)).ToList();

            if (piece.Type == ChessPieceType.King)
                RemoveDangerMoves(ref moves, piece.Owner);

            if (piece.Type == ChessPieceType.Pawn)
                AddPawnTiles(ref moves, piece);
            // remove all moves where: a piece owned by the current owner is in a specified coordinate
            // remove all moves where: an enemy piece is already blocking
            Console.WriteLine($"Moves available for {piece.Owner.ToString()} {piece.Type.ToString()}:\n{string.Join("\n", moves.Select(x => $"- {GetPosition(x.X, x.Y)}"))}");
            return moves;
        }

        private void RemoveDangerMoves(ref List<Coordinate> moves, ChessOwner player)
        {
            ChessOwner enemy = player == ChessOwner.Black ? ChessOwner.White : ChessOwner.Black;
            moves.RemoveAll(GetPossibleMoves(enemy).Contains);
        }

        private bool CanCastle(ChessOwner player)
        {
            // Check if the king has moved
            // Check if the left rook has moved
            // Check if the right rook has moved
            // Check to see if the castle moveset puts the king in check
            // Check if the king is currently in check
            // Check if the king moves through any tiles that can put the king in check
            // Check to see if there are any pieces in between the king and the rook it wants to castle with
            // A left castle moves the king 2 LEFT, and the left rook 3 RIGHT
            // A right castle moves the king 2 RIGHT, and the right rook 2 LEFT
            throw new NotImplementedException();
        }

        private List<Coordinate> GetPossibleMoves(ChessOwner player)
        {
            return GetPieces(player).SelectMany(GetBaseMoves).ToList();
        }

        public List<Coordinate> GetBaseMoves(ChessPiece piece)
            => GetBaseMoves(piece.Owner, piece.Type, piece.Rank, piece.File);

        // 0 to 7
        // file = y
        // rank = x
        private List<Coordinate> GetBaseMoves(ChessOwner owner, ChessPieceType piece, int rank, int file)
        {
            var moves = new List<Coordinate>();

            switch (piece)
            {
                case ChessPieceType.King:
                    moves.AddRange(new List<Coordinate>
                    {
                        new Coordinate(rank + 1, file),
                        new Coordinate(rank - 1, file),
                        new Coordinate(rank,     file + 1),
                        new Coordinate(rank,     file - 1),
                        new Coordinate(rank + 1, file + 1),
                        new Coordinate(rank - 1, file + 1),
                        new Coordinate(rank + 1, file - 1),
                        new Coordinate(rank - 1, file - 1)
                    });

                    break;

                case ChessPieceType.Knight:
                    moves.AddRange(new List<Coordinate>
                    {
                        new Coordinate(rank + 1, file - 2),
                        new Coordinate(rank - 1, file - 2),
                        new Coordinate(rank + 1, file + 2),
                        new Coordinate(rank - 1, file + 2),
                        new Coordinate(rank + 2, file - 1),
                        new Coordinate(rank - 2, file - 1),
                        new Coordinate(rank + 2, file + 1),
                        new Coordinate(rank - 2, file + 1)
                    });

                    break;

                case ChessPieceType.Rook:
                    MoveWest(ref moves, rank, file);
                    MoveEast(ref moves, rank, file);
                    MoveNorth(ref moves, rank, file);
                    MoveSouth(ref moves, rank, file);
                    break;

                case ChessPieceType.Bishop:
                    MoveNorthWest(ref moves, rank, file);
                    MoveNorthEast(ref moves, rank, file);
                    MoveSouthWest(ref moves, rank, file);
                    MoveSouthEast(ref moves, rank, file);
                    break;

                case ChessPieceType.Queen:
                    MoveWest(ref moves, rank, file);
                    MoveEast(ref moves, rank, file);
                    MoveNorth(ref moves, rank, file);
                    MoveSouth(ref moves, rank, file);
                    MoveNorthWest(ref moves, rank, file);
                    MoveNorthEast(ref moves, rank, file);
                    MoveSouthWest(ref moves, rank, file);
                    MoveSouthEast(ref moves, rank, file);
                    break;

                case ChessPieceType.Pawn:
                    if (owner == ChessOwner.Black)
                    {
                        moves.Add(new Coordinate(rank, file + 1));

                        if (file == 1)
                            moves.Add(new Coordinate(rank, file + 2));
                    }
                    else
                    {
                        moves.Add(new Coordinate(rank, file - 1));

                        if (file == 6)
                            moves.Add(new Coordinate(rank, file - 2));
                    }
                    break;
            }

            return moves.Where(Exclude).ToList();
        }

        private static bool Exclude(Coordinate c)
            => Exclude(c.X, c.Y);

        private static bool Exclude(int x, int y)
            => x <= Max && x >= Min && y <= Max && y >= Min;

        private void MoveWest(ref List<Coordinate> moves, int x, int y, bool includeTarget = true)
        {
            ChessPiece piece = Pieces
                .Where(p => p.Rank < x && p.File == y)
                .OrderBy(p => p.Rank - x)
                .FirstOrDefault();

            int nearest = piece?.Rank ?? Min;

            if (!includeTarget && piece != null)
                nearest++;

            while (x > nearest)
                moves.Add(new Coordinate(--x, y));
        }

        private void MoveNorth(ref List<Coordinate> moves, int x, int y, bool includeTarget = true)
        {
            ChessPiece piece = Pieces
                .Where(p => p.File < y && p.Rank == x)
                .OrderBy(p => y - p.File)
                .FirstOrDefault();

            int nearest = piece?.File ?? Min;

            if (!includeTarget && piece != null)
                nearest++;

            while (y > nearest)
                moves.Add(new Coordinate(x, --y));
        }

        private void MoveSouth(ref List<Coordinate> moves, int x, int y, bool includeTarget = true)
        {
            ChessPiece piece = Pieces
                .Where(p => p.Rank == x && p.File > y)
                .OrderBy(p => p.File - y)
                .FirstOrDefault();

            int nearest = piece?.File ?? Max;

            if (!includeTarget && piece != null)
                nearest--;

            while (y < nearest)
                moves.Add(new Coordinate(x, ++y));
        }

        private void MoveEast(ref List<Coordinate> moves, int x, int y, bool includeTarget = true)
        {
            ChessPiece piece = Pieces
                .Where(p => p.Rank > x && p.File == y)
                .OrderBy(p => x - p.Rank)
                .FirstOrDefault();

            int nearest = piece?.Rank ?? Max;

            if (!includeTarget && piece != null)
                nearest--;

            while (x < nearest)
                moves.Add(new Coordinate(++x, y));
        }

        private void MoveNorthWest(ref List<Coordinate> moves, int x, int y, bool includeTarget = true)
        {
            ChessPiece piece = Pieces
                .Where(p => x - p.Rank > 0
                            && y - p.File > 0
                            && x - p.Rank == y - p.File)
                .OrderBy(p => x - p.Rank)
                .FirstOrDefault();

            int nearestX = piece?.Rank ?? Min;
            int nearestY = piece?.File ?? Min;

            if (!includeTarget && piece != null)
            {
                nearestX++;
                nearestY++;
            }

            while (x > nearestX && y > nearestY)
                moves.Add(new Coordinate(--x, --y));
        }

        private void MoveSouthWest(ref List<Coordinate> moves, int x, int y, bool includeTarget = true)
        {
            ChessPiece piece = Pieces
                .Where(p => x - p.Rank > 0
                            && p.File - y > 0
                            && x - p.Rank == p.File - y)
                .OrderBy(p => x - p.Rank)
                .FirstOrDefault();

            int nearestX = piece?.Rank ?? Min;
            int nearestY = piece?.File ?? Max;

            if (!includeTarget && piece != null)
            {
                nearestX++;
                nearestY--;
            }

            while (x > nearestX && y < nearestY)
                moves.Add(new Coordinate(--x, ++y));
        }

        private void MoveNorthEast(ref List<Coordinate> moves, int x, int y, bool includeTarget = true)
        {
            ChessPiece piece = Pieces
                .Where(p => p.Rank - x > 0
                            && y - p.File > 0
                            && p.Rank - x == y - p.File)
                .OrderBy(p => p.Rank - x)
                .FirstOrDefault();

            int nearestX = piece?.Rank ?? Max;
            int nearestY = piece?.File ?? Min;

            if (!includeTarget && piece != null)
            {
                nearestX--;
                nearestY++;
            }

            while (x < nearestX && y > nearestY)
                moves.Add(new Coordinate(++x, --y));
        }

        private void MoveSouthEast(ref List<Coordinate> moves, int x, int y, bool includeTarget = true)
        {
            ChessPiece piece = Pieces
                .Where(p => p.Rank - x > 0
                            && p.File - y > 0
                            && p.Rank - x == p.File - y)
                .OrderBy(p => p.Rank - x)
                .FirstOrDefault();

            int nearestX = piece?.Rank ?? Max;
            int nearestY = piece?.File ?? Max;

            if (!includeTarget && piece != null)
            {
                nearestX--;
                nearestY--;
            }

            while (x < nearestX && y < nearestY)
                moves.Add(new Coordinate(++x, ++y));
        }

        public DateTime StartedAt { get; }

        public ChessMove LastMove { get; set; }

        public List<ChessPiece> Pieces { get; set; }

        public List<ChessMove> Moves { get; set; } = new List<ChessMove>();

        public void MovePiece(ChessPiece piece, Coordinate tile)
        {
            ChessPiece focus = GetPiece(piece.Rank, piece.File);

            if (focus == null)
                return;

            if (LastMove != null)
            {
                Console.WriteLine($"{LastMove.Piece} ({LastMove.Rank}, {LastMove.File} => {LastMove.To.X}, {LastMove.To.Y})");
            }

            // Um... This is disgusting lol
            if (LastMove != null
                && LastMove.Piece == ChessPieceType.Pawn
                && LastMove.Player != focus.Owner
                && LastMove.File == ((LastMove.Player == ChessOwner.Black) ? 1 : 6)
                && LastMove.To.Y == ((LastMove.Player == ChessOwner.Black) ? 3 : 4)
                && focus.Type == ChessPieceType.Pawn
                && tile.X == LastMove.To.X
                && (LastMove.Player == ChessOwner.Black
                    ? LastMove.To.Y - ((LastMove.Player == ChessOwner.Black) ? 2 : 5)
                    : ((LastMove.Player == ChessOwner.Black) ? 2 : 5) - LastMove.To.Y) == 1)
            {
                ChessPiece target = GetPiece(LastMove.To.X, LastMove.To.Y);

                if (target == null)
                    throw new Exception("Expected pawn at specified tile");

                Pieces.Remove(target);

                ChessOwner enemy = focus.Owner == ChessOwner.Black ? ChessOwner.White : ChessOwner.Black;
                ChessEvents action = 0;

                if (IsCheckmate(enemy))
                    action |= ChessEvents.Checkmate;
                else if (IsInCheck(enemy))
                    action |= ChessEvents.Check;

                LastMove = new ChessMove(StartedAt, focus, tile, action | ChessEvents.EnPassant);
                focus.Rank = tile.X;
                focus.File = tile.Y;
            }
            else
            {
                ChessPiece target = GetPiece(tile.X, tile.Y);
                ChessOwner enemy = focus.Owner == ChessOwner.Black ? ChessOwner.White : ChessOwner.Black;
                ChessEvents events = 0;

                if (IsCheckmate(enemy))
                    events |= ChessEvents.Checkmate;
                else if (IsInCheck(enemy))
                    events |= ChessEvents.Check;

                if (target != null)
                {
                    Pieces.Remove(target);
                    events |= ChessEvents.Capture;
                }

                LastMove = new ChessMove(StartedAt, focus, tile, events);
                focus.Rank = tile.X;
                focus.File = tile.Y;
            }
        }
    }
}
