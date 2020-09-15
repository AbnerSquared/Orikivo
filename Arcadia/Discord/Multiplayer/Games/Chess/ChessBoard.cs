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
                        Piece = ChessRank.Rook,
                        Rank = 0,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Knight,
                        Rank = 1,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Bishop,
                        Rank = 2,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Queen,
                        Rank = 3,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.King,
                        Rank = 4,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Bishop,
                        Rank = 5,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Knight,
                        Rank = 6,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Rook,
                        Rank = 7,
                        File = 0
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Pawn,
                        Rank = 0,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Pawn,
                        Rank = 1,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Pawn,
                        Rank = 2,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Pawn,
                        Rank = 3,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Pawn,
                        Rank = 4,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Pawn,
                        Rank = 5,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Pawn,
                        Rank = 6,
                        File = 1
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.Black,
                        Piece = ChessRank.Pawn,
                        Rank = 7,
                        File = 1
                    },

                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Rook,
                        Rank = 0,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Knight,
                        Rank = 1,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Bishop,
                        Rank = 2,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Queen,
                        Rank = 3,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.King,
                        Rank = 4,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Bishop,
                        Rank = 5,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Knight,
                        Rank = 6,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Rook,
                        Rank = 7,
                        File = 7
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Pawn,
                        Rank = 0,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Pawn,
                        Rank = 1,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Pawn,
                        Rank = 2,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Pawn,
                        Rank = 3,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Pawn,
                        Rank = 4,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Pawn,
                        Rank = 5,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Pawn,
                        Rank = 6,
                        File = 6
                    },
                    new ChessPiece
                    {
                        Owner = ChessOwner.White,
                        Piece = ChessRank.Pawn,
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
                var pieceType = (ChessRank)RandomProvider.Instance.Next(1, 7);

                pieces.Add(new ChessPiece
                {
                    Owner = i < whitePieceCount ? ChessOwner.White : ChessOwner.Black,
                    Piece = pieceType,
                    File = tile.X,
                    Rank = tile.Y
                });
            }

            Console.WriteLine($"Pieces Added:\n{string.Join("\n", pieces.Select(p => $"- {p.Owner.ToString()} {p.Piece.ToString()}"))}");

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

        public string DrawBoard(ChessOwner perspective)
        {
            var text = new StringBuilder();
            text.AppendLine("```");
            text.AppendLine(DrawRanks(perspective));

            bool isFlipped = perspective == ChessOwner.Black;

            for (int column = isFlipped ? Max : Min; isFlipped ? column >= Min : column <= Max; column += isFlipped ? -1 : 1)
            {
                text.Append(column + 1);

                for (int row = isFlipped ? Max : Min; isFlipped ? row >= Min : row <= Max; row += isFlipped ? -1 : 1)
                {
                    ChessPiece piece = GetPiece(row, column);
                    string tile = piece == null ? EmptyTile
                        : ChessPiece.GetString(piece.Piece, piece.Owner != perspective);
                    text.Append($" {tile}");
                }

                text.AppendLine();
            }

            text.AppendLine("```");
            return text.ToString();
        }

        public string DrawMoves(ChessPiece focus, ChessOwner perspective)
        {
            List<Coordinate> moves = GetMoves(focus);
            var text = new StringBuilder();
            text.AppendLine("```");
            text.AppendLine(DrawRanks(perspective));

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
                            : ChessPiece.GetString(piece.Piece, piece.Owner != perspective);
                    text.Append($" {tile}");
                }

                text.AppendLine();
            }

            text.AppendLine("```");
            return text.ToString();
        }

        private static string DrawRanks(ChessOwner perspective)
        {
            return perspective == ChessOwner.Black
                ? "- H G F E D C B A"
                : "- A B C D E F G H";
        }

        public bool IsInCheck(ChessOwner player)
        {
            ChessPiece king = GetPieces(player).FirstOrDefault(x => x.Piece == ChessRank.King);

            if (king == null)
                throw new Exception("Expected a king piece");

            ChessOwner enemy = player == ChessOwner.Black ? ChessOwner.White : ChessOwner.Black;
            return GetPossibleMoves(enemy).Any(x => x.X == king.Rank && x.Y == king.File);
        }

        public bool IsCheckmate(ChessOwner player)
        {
            if (!IsInCheck(player))
                return false;

            ChessPiece king = GetPieces(player).FirstOrDefault(x => x.Piece == ChessRank.King);

            ChessOwner enemy = player == ChessOwner.Black ? ChessOwner.White : ChessOwner.Black;
            return GetPossibleMoves(enemy).Any(x => x.X == king.Rank && x.Y == king.File);
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

        public List<Coordinate> GetMoves(ChessPiece piece)
        {
            //if (IsInCheck(piece.Owner))
            //    return GetSafeMoves(piece.Owner);

            var moves = GetBaseMoves(piece.Owner, piece.Piece, piece.Rank, piece.File)
                .Where(x => !GetUsedTiles(piece.Owner).Contains(x)).ToList();

            if (piece.Piece == ChessRank.King)
                RemoveDangerMoves(ref moves, piece.Owner);

            if (piece.Piece == ChessRank.Pawn)
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
            }
            // remove all moves where: a piece owned by the current owner is in a specified coordinate
            // remove all moves where: an enemy piece is already blocking
            Console.WriteLine($"Moves available for {piece.Owner.ToString()} {piece.Piece.ToString()}:\n{string.Join("\n", moves.Select(x => $"- {GetPosition(x.X, x.Y)}"))}");
            return moves;
        }

        private void RemoveDangerMoves(ref List<Coordinate> moves, ChessOwner player)
        {
            ChessOwner enemy = player == ChessOwner.Black ? ChessOwner.White : ChessOwner.Black;
            moves.RemoveAll(GetPossibleMoves(enemy).Contains);
        }

        private List<Coordinate> GetSafeMoves(ChessOwner player)
        {
            return null;
        }

        private List<ChessPiece> GetProtectPieces(ref List<ChessPiece> dangerPieces, ChessOwner player)
        {
            ChessPiece king = GetPieces(player).FirstOrDefault(x => x.Piece == ChessRank.King);

            if (king == null)
                throw new Exception("Expected a king piece");

            return null;
        }

        private List<ChessPiece> GetCheckPieces(ChessOwner player)
        {
            ChessOwner enemy = player == ChessOwner.Black ? ChessOwner.White : ChessOwner.Black;
            ChessPiece king = GetPieces(player).FirstOrDefault(x => x.Piece == ChessRank.King);

            if (king == null)
                throw new Exception("Expected a king piece");

            return GetPieces(enemy)
                .Where(x => GetBaseMoves(x.Owner, x.Piece, x.Rank, x.File).Contains(new Coordinate(king.Rank, king.File))).ToList();
        }

        private List<Coordinate> GetPossibleMoves(ChessOwner player)
        {
            return GetPieces(player).SelectMany(p => GetBaseMoves(p.Owner, p.Piece, p.Rank, p.File)).ToList();
        }

        // 0 to 7
        // file = y
        // rank = x
        private List<Coordinate> GetBaseMoves(ChessOwner owner, ChessRank piece, int rank, int file)
        {
            var moves = new List<Coordinate>();

            switch (piece)
            {
                case ChessRank.King:
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

                case ChessRank.Knight:
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

                case ChessRank.Rook:
                    MoveWest(ref moves, rank, file, owner);
                    MoveEast(ref moves, rank, file, owner);
                    MoveNorth(ref moves, rank, file, owner);
                    MoveSouth(ref moves, rank, file, owner);
                    break;

                case ChessRank.Bishop:
                    MoveNorthWest(ref moves, rank, file, owner);
                    MoveNorthEast(ref moves, rank, file, owner);
                    MoveSouthWest(ref moves, rank, file, owner);
                    MoveSouthEast(ref moves, rank, file, owner);
                    break;

                case ChessRank.Queen:
                    MoveWest(ref moves, rank, file, owner);
                    MoveEast(ref moves, rank, file, owner);
                    MoveNorth(ref moves, rank, file, owner);
                    MoveSouth(ref moves, rank, file, owner);
                    MoveNorthWest(ref moves, rank, file, owner);
                    MoveNorthEast(ref moves, rank, file, owner);
                    MoveSouthWest(ref moves, rank, file, owner);
                    MoveSouthEast(ref moves, rank, file, owner);
                    break;

                case ChessRank.Pawn:
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

        private void MoveWest(ref List<Coordinate> moves, int x, int y, ChessOwner player)
        {
            ChessPiece piece = Pieces
                .Where(p => p.Rank < x && p.File == y)
                .OrderBy(p => p.Rank - x)
                .FirstOrDefault();

            int nearest = piece?.Rank ?? Min;
            bool isEnemy = piece?.Owner != player;

            while (x > nearest)
                moves.Add(new Coordinate(--x, y));
        }

        private void MoveNorth(ref List<Coordinate> moves, int x, int y, ChessOwner player)
        {
            ChessPiece piece = Pieces
                .Where(p => p.File < y && p.Rank == x)
                .OrderBy(p => y - p.File)
                .FirstOrDefault();

            int nearest = piece?.File ?? Min;
            bool isEnemy = piece?.Owner != player;

            while (y > nearest)
                moves.Add(new Coordinate(x, --y));
        }

        private void MoveSouth(ref List<Coordinate> moves, int x, int y, ChessOwner player)
        {
            ChessPiece piece = Pieces
                .Where(p => p.Rank == x && p.File > y)
                .OrderBy(p => p.File - y)
                .FirstOrDefault();

            int nearest = piece?.File ?? Max;
            bool isEnemy = piece?.Owner != player;

            while (y < nearest)
                moves.Add(new Coordinate(x, ++y));
        }

        private void MoveEast(ref List<Coordinate> moves, int x, int y, ChessOwner player)
        {
            ChessPiece piece = Pieces
                .Where(p => p.Rank > x && p.File == y)
                .OrderBy(p => x - p.Rank)
                .FirstOrDefault();

            int nearest = piece?.Rank ?? Max;
            bool isEnemy = piece?.Owner != player;

            while (x < nearest)
                moves.Add(new Coordinate(++x, y));
        }

        private void MoveNorthWest(ref List<Coordinate> moves, int x, int y, ChessOwner player)
        {
            ChessPiece piece = Pieces
                .Where(p => x - p.Rank > 0
                            && y - p.File > 0
                            && x - p.Rank == y - p.File)
                .OrderBy(p => x - p.Rank)
                .FirstOrDefault();

            int nearestX = piece?.Rank ?? Min;
            int nearestY = piece?.File ?? Min;
            bool isEnemy = piece?.Owner != player;

            while (x > nearestX && y > nearestY)
                moves.Add(new Coordinate(--x, --y));
        }

        private void MoveSouthWest(ref List<Coordinate> moves, int x, int y, ChessOwner player)
        {
            ChessPiece piece = Pieces
                .Where(p => x - p.Rank > 0
                            && p.File - y > 0
                            && x - p.Rank == p.File - y)
                .OrderBy(p => x - p.Rank)
                .FirstOrDefault();

            int nearestX = piece?.Rank ?? Min;
            int nearestY = piece?.File ?? Max;
            bool isEnemy = piece?.Owner != player;

            while (x > nearestX && y < nearestY)
                moves.Add(new Coordinate(--x, ++y));
        }

        private void MoveNorthEast(ref List<Coordinate> moves, int x, int y, ChessOwner player)
        {
            ChessPiece piece = Pieces
                .Where(p => p.Rank - x > 0
                            && y - p.File > 0
                            && p.Rank - x == y - p.File)
                .OrderBy(p => p.Rank - x)
                .FirstOrDefault();

            int nearestX = piece?.Rank ?? Max;
            int nearestY = piece?.File ?? Min;
            bool isEnemy = piece?.Owner != player;

            while (x < nearestX && y > nearestY)
                moves.Add(new Coordinate(++x, --y));
        }

        private void MoveSouthEast(ref List<Coordinate> moves, int x, int y, ChessOwner player)
        {
            ChessPiece piece = Pieces
                .Where(p => p.Rank - x > 0
                            && p.File - y > 0
                            && p.Rank - x == p.File - y)
                .OrderBy(p => p.Rank - x)
                .FirstOrDefault();

            int nearestX = piece?.Rank ?? Max;
            int nearestY = piece?.File ?? Max;
            bool isEnemy = piece?.Owner != player;

            while (x < nearestX && y < nearestY)
                moves.Add(new Coordinate(++x, ++y));
        }

        public List<ChessPiece> Pieces { get; set; }

        public void MovePiece(ChessPiece piece, Coordinate tile)
        {
            ChessPiece focus = GetPiece(piece.Rank, piece.File);

            if (focus == null)
                return;

            focus.Rank = tile.X;
            focus.File = tile.Y;
        }
    }
}