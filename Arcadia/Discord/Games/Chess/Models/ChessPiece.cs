using System;
using Orikivo.Text;

namespace Arcadia.Multiplayer.Games
{
    public class ChessPiece
    {
        public static bool TryParse(string input, out ChessPieceType piece)
        {
            piece = 0;

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "p", "pawn"))
            {
                piece = ChessPieceType.Pawn;
                return true;
            }

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "n", "knight"))
            {
                piece = ChessPieceType.Knight;
                return true;
            }

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "b", "bishop"))
            {
                piece = ChessPieceType.Bishop;
                return true;
            }

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "r", "rook"))
            {
                piece = ChessPieceType.Rook;
                return true;
            }

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "q", "queen"))
            {
                piece = ChessPieceType.Queen;
                return true;
            }

            if (input.EqualsAny(StringComparison.OrdinalIgnoreCase, "k", "king"))
            {
                piece = ChessPieceType.King;
                return true;
            }

            return false;
        }

        // TODO: Move to a formatter
        public static string GetString(ChessPieceType rank, ChessOwner player, ChessIconFormat format = ChessIconFormat.Text)
        {
            bool isBlack = player == ChessOwner.Black;
            bool isEmote = format == ChessIconFormat.Emote;
            string text = rank switch
            {
                ChessPieceType.Pawn when isEmote => isBlack ? "♙" : "♟",
                ChessPieceType.Pawn => "P",
                ChessPieceType.Knight when isEmote => isBlack ? "♘" : "♞",
                ChessPieceType.Knight => "N",
                ChessPieceType.Bishop when isEmote => isBlack ? "♗" : "♝",
                ChessPieceType.Bishop => "B",
                ChessPieceType.Rook when isEmote => isBlack ? "♖" : "♜",
                ChessPieceType.Rook => "R",
                ChessPieceType.Queen when isEmote => isBlack ? "♕" : "♛",
                ChessPieceType.Queen => "Q",
                ChessPieceType.King when isEmote => isBlack ? "♔" : "♚",
                ChessPieceType.King => "K",
                _ => throw new ArgumentException("Unknown rank")
            };

            if (isBlack && !isEmote)
                text = text.ToLower();

            return text;
        }

        /// <summary>
        /// Represents the type that this <see cref="ChessPiece"/> represents.
        /// </summary>
        public ChessPieceType Type { get; internal set; }

        /// <summary>
        /// Represents the x-coordinate of this <see cref="ChessPiece"/>.
        /// </summary>
        public int Rank { get; internal set; }

        /// <summary>
        /// Represents the y-coordinate of this <see cref="ChessPiece"/>.
        /// </summary>
        public int File { get; internal set; }

        /// <summary>
        /// Represents the owner of this <see cref="ChessPiece"/>.
        /// </summary>
        public ChessOwner Owner { get; internal set; }
    }
}
