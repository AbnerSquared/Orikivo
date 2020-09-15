using System;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia.Multiplayer.Games
{
    public class ChessPiece
    {
        public static bool TryParse(string input, out ChessRank piece)
        {
            piece = 0;

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "p", "pawn"))
            {
                piece = ChessRank.Pawn;
                return true;
            }

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "n", "knight"))
            {
                piece = ChessRank.Knight;
                return true;
            }

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "b", "bishop"))
            {
                piece = ChessRank.Bishop;
                return true;
            }

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "r", "rook"))
            {
                piece = ChessRank.Rook;
                return true;
            }

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "q", "queen"))
            {
                piece = ChessRank.Queen;
                return true;
            }

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "k", "king"))
            {
                piece = ChessRank.King;
                return true;
            }

            return false;
        }

            public static string GetString(ChessRank rank, bool isEnemy = false)
        {
            string text = rank switch
            {
                ChessRank.Pawn => "P",
                ChessRank.Knight => "N",
                ChessRank.Bishop => "B",
                ChessRank.Rook => "R",
                ChessRank.Queen => "Q",
                ChessRank.King => "K",
                _ => throw new ArgumentException("Unknown rank")
            };

            if (isEnemy)
                text = text.ToLower();

            return text;
        }

        public Coordinate StartingPosition { get; set; }

        public ChessRank Piece { get; internal set; }

        public int File { get; internal set; }

        public int Rank { get; internal set; }

        public ChessOwner Owner { get; internal set; }
    }
}