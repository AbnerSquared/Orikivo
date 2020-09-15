using System;
using System.Text;

namespace Arcadia.Multiplayer.Games
{
    public class ChessMove
    {
        internal ChessMove(DateTime startedAt, ChessRank piece, int file, int rank, ChessMoveAction action)
        {
            StartedAt = startedAt;
            Piece = piece;
            File = file;
            Rank = rank;
            Action = action;
            Timestamp = DateTime.UtcNow;
        }

        private DateTime StartedAt { get; }

        public ChessRank Piece { get; }

        public int File { get; }

        public int Rank { get; }

        public DateTime Timestamp { get; }

        // Actions made during this move
        public ChessMoveAction Action { get; }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append($"[{(DateTime.UtcNow - Timestamp)}] {ChessPiece.GetString(Piece).ToUpper()}{ChessBoard.GetPosition(File, Rank)}");

            return result.ToString();
        }
    }
}