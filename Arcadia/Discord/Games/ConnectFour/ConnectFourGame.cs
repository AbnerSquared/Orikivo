using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Multiplayer.Games
{
    public class ConnectFourGame
    {
        /*
         Game Properties:
         CurrentPlayer: The player that is currently making a move
         Board: The state of the game board

         
         */
    }

    public enum ConnectFourPiece
    {
        None = 0,
        Red = 1,
        Blue = 2
    }

    public class ConnectFourSlot
    {
        public ConnectFourPiece Piece { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }
    }

    public class ConnectFourBoard
    {
        public List<ConnectFourSlot> Slots { get; set; }

        private int GetLowestColumn(int row)
        {
            return Slots.Count(x => x.Row == row) + 1; // 3 pieces in row 1, slot 3 is the lowest column, as 4, 5, and 6 are taken
        }

        public void SetSlot(int row, ConnectFourPiece piece)
        {
            // Handle if rows are overflowed
            Slots.Add(new ConnectFourSlot()
            {
                Column = GetLowestColumn(row),
                Row = row,
                Piece = piece
            });
        }
    }
}
