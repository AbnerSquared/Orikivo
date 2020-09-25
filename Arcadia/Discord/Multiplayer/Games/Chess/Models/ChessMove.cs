using System;
using System.Text;
using Orikivo.Drawing;

namespace Arcadia.Multiplayer.Games
{
    public class ChessMove
    {
        internal ChessMove(DateTime startedAt, ChessPiece piece, Coordinate to,  ChessMoveAction action)
        {
            StartedAt = startedAt;
            Player = piece.Owner;
            Piece = piece.Piece;
            File = piece.File;
            Rank = piece.Rank;
            To = to;
            Action = action;
            Timestamp = DateTime.UtcNow;
        }

        private DateTime StartedAt { get; }

        public ChessOwner Player { get; }

        public ChessRank Piece { get; }

        public int File { get; }

        public int Rank { get; }

        public Coordinate To { get; }

        public DateTime Timestamp { get; }

        // Actions made during this move
        public ChessMoveAction Action { get; }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append($"[{(DateTime.UtcNow - Timestamp)}] {ChessPiece.GetString(Piece, Player, ChessIconFormat.Text).ToUpper()}{ChessBoard.GetPosition(File, Rank)}");

            return result.ToString();
        }
    }
}