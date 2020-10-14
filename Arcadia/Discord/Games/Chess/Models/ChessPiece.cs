using System;
using Orikivo.Drawing;
using Orikivo.Text;

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

            public static string GetString(ChessRank rank, ChessOwner player, ChessIconFormat format = ChessIconFormat.Text)
        {
            bool isBlack = player == ChessOwner.Black;
            bool isEmote = format == ChessIconFormat.Emote;
            string text = rank switch
            {
                ChessRank.Pawn when isEmote => isBlack ? "♙" : "♟",
                ChessRank.Pawn => "P",
                ChessRank.Knight when isEmote => isBlack ? "♘" : "♞",
                ChessRank.Knight => "N",
                ChessRank.Bishop when isEmote => isBlack ? "♗" : "♝",
                ChessRank.Bishop => "B",
                ChessRank.Rook when isEmote => isBlack ? "♖" : "♜",
                ChessRank.Rook => "R",
                ChessRank.Queen when isEmote => isBlack ? "♕" : "♛",
                ChessRank.Queen => "Q",
                ChessRank.King when isEmote => isBlack ? "♔" : "♚",
                ChessRank.King => "K",
                _ => throw new ArgumentException("Unknown rank")
            };

            if (isBlack && !isEmote)
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