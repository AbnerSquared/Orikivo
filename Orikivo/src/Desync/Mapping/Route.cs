using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    // TODO: Just use Points to calculate,
    // instead of having 3 separate properties.
    // TODO: Create GetCurrentPosition(), which returns a Vector2 based on which the time it was initiated.
    /// <summary>
    /// Represents an enforced <see cref="Path"/>.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Where the <see cref="Route"/> was initiated.
        /// </summary>
        public Vector2 From { get; set; }

        // If there are no points, the position from => to is calculated.
        public List<Vector2> Points { get; set; } = new List<Vector2>();

        /// <summary>
        /// Where the <see cref="Route"/> is leading to.
        /// </summary>
        public Vector2 To { get; set; }

        public TransportType Transport { get; set; }

        public float TravelSpeed { get; set; }

        // If true, this route has to be taken from the start.
        // otherwise, the player will travel to the nearest point before going the rest of that route.
        public bool Enforce { get; set; }

        public float Length => GetTotalDistance();
        public TimeSpan Time => GetTime();

        // TODO: Implement travel speed calculation.
        public Vector2 GetCurrentPosition(DateTime startedAt)
        {
            float elapsedSeconds = (float)((DateTime.UtcNow - startedAt).TotalSeconds);

            if (elapsedSeconds > GetTotalTime())
                return To;

            // a float from 0 to 1. this helps correlate position
            float progress = RangeF.Convert(0.0f, GetTotalTime(), 0.0f, 1.0f, elapsedSeconds);


            return GetPositionAt(progress);
        }

        // gets the desired point based on the completion of this route.
        private Vector2 GetPositionAt(float progress)
        {
            // converts the progress into the total distance travelled based on progress.
            float travelled = RangeF.Convert(0.0f, 1.0f, 0.0f, GetTotalDistance(), progress);

            float dist = 0.0f;
            Vector2 from = From;
            int i = 0;

            // TODO: Create a sub-list referencing Points, and simply insert the From/To values
            foreach (Vector2 to in Points)
            {
                dist += CalcF.Distance(from, to);
                from = to;

                if (dist >= travelled)
                    break;

                i++;

                
            }

            if (Points.Count == 0) // if there aren't any points specified
            {
                float pathProgress = RangeF.Convert(0.0f, GetTotalDistance(), 0.0f, 1.0f, travelled);

                return new Vector2(RangeF.Convert(0.0f, 1.0f, From.X, To.X, pathProgress),
                    RangeF.Convert(0.0f, 1.0f, From.Y, To.Y, pathProgress));
            }
            else if (i == 0) // if the total travelled was less than the first checkpoint.
            {
                float pathProgress = RangeF.Convert(0.0f, GetDistanceAt(i), 0.0f, 1.0f, travelled);

                return new Vector2(RangeF.Convert(0.0f, 1.0f, From.X, Points[i].X, pathProgress),
                    RangeF.Convert(0.0f, 1.0f, From.Y, Points[i].Y, pathProgress));
            }
            else if (dist < travelled) // if the total travelled is greater than all points
            {
                dist += CalcF.Distance(from, To);

                if (travelled >= dist)
                    return To;
                else
                {
                    float pathProgress = RangeF.Convert(GetDistanceAt(i), GetTotalDistance(), 0.0f, 1.0f, travelled);

                    return new Vector2(RangeF.Convert(0.0f, 1.0f, Points[i].X, To.X, pathProgress),
                        RangeF.Convert(0.0f, 1.0f, Points[i].Y, To.Y, pathProgress));
                }
            }
            else // if the travelled amount if within the points specified.
            {
                float pathProgress = RangeF.Convert(GetDistanceAt(i - 1), GetDistanceAt(i), 0.0f, 1.0f, travelled);

                return new Vector2(RangeF.Convert(0.0f, 1.0f, Points[i - 1].X, Points[i].X, pathProgress),
                    RangeF.Convert(0.0f, 1.0f, Points[i - 1].Y, Points[i].Y, pathProgress));
            }
        }

        // gets the distance travelled for the specified index of the point.
        public float GetDistanceAt(int index)
        {
            float dist = 0.0f;
            Vector2 from = From;
            int i = 0;
            foreach (Vector2 to in Points)
            {
                if (index == i)
                    break;


                dist += CalcF.Distance(from, to);
                from = to;
                i++;
            }

            return dist;
        }

        // this sets up a new route based on the user's current position.
        public Route NewFromPosition(Vector2 pos)
        {
            Route newRoute = new Route();
            List<Vector2> points = new List<Vector2>();
            if (Enforce)
            {
                newRoute.From = pos;
                points.Add(From);
                points.AddRange(Points);
            }
            else
            {
                newRoute.From = pos;
                int index = ClosestPointAt(pos);
                points.Add(Points[index]);
                points.AddRange(Points.Skip(index + 1));
                
            }

            newRoute.Points = points;
            newRoute.To = To;

            return newRoute;
        }

        // this gets the index of the closest point based on a current position.
        private int ClosestPointAt(Vector2 pos)
        {
            float dist = 0.0f;
            int index = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                float amt = CalcF.Distance(pos, Points[i]);

                if (amt < dist)
                {
                    dist = amt;
                    index = i;
                }
                else if (amt == 0)
                    return i;
            }

            return index;
        }

        // if the player is off-road, this accounts for that.
        private float GetTotalTime(Vector2 pos)
        {
            float time = 0.0f;
            // the index of the closest point. this helps determine how many points should be skipped.
            int index = ClosestPointAt(pos);
            Vector2 last = Points[index];

            // this skips all points up to the closest point, including that point.
            IEnumerable<Vector2> points = Points.Skip(index + 1);

            time += GetTime(pos, last, false);

            foreach (Vector2 point in points)
            {
                time += GetTime(last, point);
                last = point;
            }

            time += GetTime(last, To);

            return time;
        }

        private float GetSpeed(bool isPath)
        {
            float scale = Engine.GetBaseSpeedAt(LocationType.Sector);
            float boost = isPath ? 1.10f : 1.00f;
            float speed = 10.0f;

            return speed * scale * boost;
        }

        private float GetTime(Vector2 a, Vector2 b, bool isPath = true)
        {
            float speed = GetSpeed(isPath);     
            float dist = CalcF.Distance(a, b);
            return dist / speed;
        }
        
        // this is the true total time, if the player were to be travelling on the route.
        private float GetTotalTime()
        {
            float time = 0.0f;
            Vector2 last = From;
            
            foreach (Vector2 point in Points)
            {
                // TIME += sqrt((b.x - a.x)^2 + (b.y - a.y)^2) / (60.0 * 0.25 * 1.25)
                time += GetTime(last, point);
                last = point;
            }

            time += GetTime(last, To);

            return time;
        }

        public TimeSpan GetTime()
        {
            return TimeSpan.FromSeconds(GetTotalTime());
        }

        private float GetTotalDistance()
        {
            float dist = 0.0f;
            Vector2 from = From;
            foreach (Vector2 to in Points)
            {
                dist += CalcF.Distance(from, to);
                from = to;
            }

            dist += CalcF.Distance(from, To);
            return dist;
        }
    }
}
