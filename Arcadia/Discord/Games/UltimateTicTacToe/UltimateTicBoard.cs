using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Framework;

namespace Arcadia.Multiplayer.Games
{
    public class UltimateTicBoard
    {
        public static UltimateTicBoard GetDefault()
        {
            return new UltimateTicBoard
            {
                Boards = new List<TicBoard>
                {
                    new TicBoard(TicDirection.TopLeft),
                    new TicBoard(TicDirection.Top),
                    new TicBoard(TicDirection.TopRight),
                    new TicBoard(TicDirection.CenterLeft),
                    new TicBoard(TicDirection.Center),
                    new TicBoard(TicDirection.CenterRight),
                    new TicBoard(TicDirection.BottomLeft),
                    new TicBoard(TicDirection.Bottom),
                    new TicBoard(TicDirection.BottomRight)
                }
            };
        }

        public List<TicBoard> Boards { get; set; }

        public TicBoard GetBoard(TicDirection direction)
        {
            return Boards.FirstOrDefault(x => x.Segment == direction);
        }

        public IEnumerable<TicBoard> GetOpenBoards()
        {
            return Boards.Where(x => !x.ContainsWinner());
        }

        public bool CheckWinState(TicMarker marker)
        {
            IEnumerable<int> segments = Boards
                .Where(x => x.CheckWinState(marker))
                .Select(y => (int)y.Segment);

            foreach (List<int> winnable in TicBoard.GetWinningPairs())
            {
                if (winnable.All(x => segments.Contains(x)))
                    return true;
            }

            return false;
        }

        public string Draw()
        {
            var result = new StringBuilder();


            result.AppendLine("```");
            int s = 0;
            var rows = new List<string> { "", "", "", "", "", "", "", "", "" };

            foreach (IEnumerable<TicBoard> set in Boards.Group(3))
            {
                int b = 0;
                int offset = 3 * s;
                foreach (TicBoard board in set)
                {
                    int r = 0;
                    // Logger.Debug($"R:{r} B:{b} OFFSET:{offset} S:{s}");

                    foreach (string row in board.DrawRows())
                    {
                        if (b != 0)
                            rows[offset + r] += "   ";

                        rows[offset + r] += row;
                        r++;
                    }
                    b++;
                }

                s++;
            }

            int i = 0;
            foreach (string row in rows)
            {
                if (i >= 3)
                {
                    result.AppendLine();
                    i = 0;
                }

                result.AppendLine(row);
                i++;
            }

            result.Append("```");

            return result.ToString();
        }
    }
}