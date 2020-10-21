using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Multiplayer.Games
{
    public class TicBoard
    {
        public TicBoard() { }

        public TicBoard(TicDirection direction)
        {
            Segment = direction;
            Slots = GetDefault().Slots;
        }

        public static TicBoard GetDefault()
        {
            return new TicBoard
            {
                Slots = new List<TicSlot>
            {
                new TicSlot
                {
                    Segment = TicDirection.TopLeft,
                    Marker = 0
                },
                new TicSlot
                {
                    Segment = TicDirection.Top,
                    Marker = 0
                },
                new TicSlot
                {
                    Segment = TicDirection.TopRight,
                    Marker = 0
                },
                new TicSlot
                {
                    Segment = TicDirection.CenterLeft,
                    Marker = 0
                },
                new TicSlot
                {
                    Segment = TicDirection.Center,
                    Marker = 0
                },
                new TicSlot
                {
                    Segment = TicDirection.CenterRight,
                    Marker = 0
                },
                new TicSlot
                {
                    Segment = TicDirection.BottomLeft,
                    Marker = 0
                },
                new TicSlot
                {
                    Segment = TicDirection.Bottom,
                    Marker = 0
                },
                new TicSlot
                {
                    Segment = TicDirection.BottomRight,
                    Marker = 0
                }
            }
            };
        }

        public TicDirection Segment { get; set; }
        public List<TicSlot> Slots { get; set; }

        public IEnumerable<TicDirection> GetOpenDirections()
        {
            return Slots.Where(x => x.Marker == 0).Select(x => x.Segment);
        }

        public static List<List<int>> GetWinningPairs()
        {
            return new List<List<int>>
            {
                new List<int>
                {
                    1, 2, 3
                },
                new List<int>
                {
                    4, 5, 6
                },
                new List<int>
                {
                    7, 8, 9
                },
                new List<int>
                {
                    1, 4, 7
                },
                new List<int>
                {
                    2, 5, 8
                },
                new List<int>
                {
                    3, 6, 9
                },
                new List<int>
                {
                    1, 5, 9
                },
                new List<int>
                {
                    3, 5, 7
                },
            };
        }

        public bool SetMarker(TicMarker marker, TicDirection direction)
        {
            if (marker == 0)
                return false;

            if (!GetOpenDirections().Contains(direction))
                return false;

            Slots.First(x => x.Segment == direction).Marker = marker;
            return true;
        }

        public bool ContainsWinner()
        {
            return CheckWinState(TicMarker.Circle) || CheckWinState(TicMarker.Cross);
        }

        public TicMarker GetWinner()
        {
            return CheckWinState(TicMarker.Circle) ? TicMarker.Circle : CheckWinState(TicMarker.Cross) ? TicMarker.Cross : (TicMarker)0;
        }

        // Check to see if a marker has won
        public bool CheckWinState(TicMarker marker)
        {
            IEnumerable<int> segments = Slots.Where(x => x.Marker == marker).Select(x => (int)x.Segment);

            foreach (List<int> winnable in GetWinningPairs())
            {
                if (winnable.All(x => segments.Contains(x)))
                    return true;
            }

            return false;

            // Winning matches:
            /*
                Board:
                1 2 3
                4 5 6
                7 8 9

                Winning matches:
                123, 456, 789, 147, 258, 369, 159, 357
             */
        }

        public static string DrawMarker(TicMarker marker)
        {
            return marker == TicMarker.Cross ? "x" : marker == TicMarker.Circle ? "o" : "-";
        }

        public string Draw()
        {
            var result = new StringBuilder();

            result.AppendLine("```");

            int c = 0;
            foreach (TicSlot slot in Slots.OrderBy(x => (int)x.Segment))
            {
                if (c >= 3)
                {
                    result.AppendLine();
                    c = 0;
                }
                else if (c != 0)
                {
                    result.Append(" ");
                }

                result.Append(DrawMarker(slot.Marker));
                c++;
            }

            result.AppendLine();
            result.Append("```");
            return result.ToString();
        }

        public IEnumerable<string> DrawRows()
        {
            TicMarker winner = GetWinner();

            if (winner != 0)
            {
                return DrawLargeMarker(winner);
            }

            var rows = new List<string>();

            for (int i = 0; i < 3; i++)
                rows.Add(DrawRow(i));

            return rows;
        }

        private IEnumerable<string> DrawLargeMarker(TicMarker marker)
        {
            var rows = new List<string>();

            switch (marker)
            {
                case TicMarker.Circle:
                    rows.Add("o o o");
                    rows.Add("o   o");
                    rows.Add("o o o");
                    return rows;

                case TicMarker.Cross:
                    rows.Add("x   x");
                    rows.Add("  x  ");
                    rows.Add("x   x");
                    return rows;

                default:
                    throw new Exception("Unknown marker was specified");
            }
        }

        public string DrawRow(int i)
        {
            var result = new StringBuilder();
            int offset = 3 * i;

            if (i > 2 || i < 0)
                throw new Exception("The specified index is out of bounds");

            int len = 0;
            foreach (TicSlot slot in Slots.OrderBy(x => (int)x.Segment).Skip(offset))
            {
                if (len >= 3)
                    break;

                if (len != 0)
                    result.Append(" ");

                result.Append(DrawMarker(slot.Marker));
                len++;
            }

            return result.ToString();
        }
    }
}
