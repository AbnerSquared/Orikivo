using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    // this handles all of the math functions
    public static partial class Engine
    {
        internal static Line GetLineToRegion(Vector2 point, RegionF region)
            => GetLineToRegion(point.X, point.Y, region);

        internal static Line GetLineToRegion(float x, float y, RegionF region)
        {
            Line line = new Line(x, y, region.Midpoint.X, region.Midpoint.Y);
            Vector2? intersection = line.GetClosestIntersection(region);

            if (intersection.HasValue)
                line.B = intersection.Value;

            return line;
        }

        internal static EntityHitbox CreateHitbox(float x, float y, float sight, float reach, LocationType type = LocationType.World)
            => new EntityHitbox(x, y, sight * GetTravelScale(type), reach * GetTravelScale(type));

        internal static float GetBaseSpeed()
            => 1.0f / (TimePerPixel * World.Scale);

        internal static float GetBaseSpeedIn(LocationType type)
            => 1.0f / (TimePerPixel * World.Scale * GetTravelScale(type));

        internal static float GetTravelScale(LocationType type)
            => type switch
            {
                LocationType.World => 1.0f,
                LocationType.Field => 0.5f,
                LocationType.Sector => 0.25f,
                _ => throw new ArgumentException("Invalid LocationType specified")
            };

        internal static bool CanSeeRegion(EntityHitbox hitbox, RegionF region)
            => hitbox.Sight.Intersects(region);

        internal static bool IsNearRegion(EntityHitbox hitbox, RegionF region)
            => hitbox.Reach.Intersects(region);

        internal static bool IsNearPoint(EntityHitbox hitbox, float x, float y)
            => hitbox.Reach.Contains(x, y);

        internal static bool IsNearPoint(EntityHitbox hitbox, Vector2 point)
            => hitbox.Reach.Contains(point);

        internal static bool CanSeePoint(EntityHitbox hitbox, float x, float y)
            => hitbox.Sight.Contains(x, y);

        internal static bool CanSeePoint(EntityHitbox hitbox, Vector2 point)
            => hitbox.Sight.Contains(point);

        internal static float GetSpeed(float baseSpeed, bool isPath)
        {
            // get location type
            float scale = GetBaseSpeedIn(LocationType.Sector);
            float boost = isPath ? 1.10f : 1.00f;
            float speed = baseSpeed;

            return baseSpeed * scale * boost;
        }

        internal static float GetTime(float dist, float baseSpeed, bool isPath)
        {
            float speed = GetSpeed(baseSpeed, isPath);
            return dist / speed;
        }

        internal static float GetTime(Vector2 from, Vector2 to, float baseSpeed, bool isPath)
            => GetTime(GetDistanceBetween(from, to), baseSpeed, isPath);

        internal static float GetDistanceBetween(Vector2 from, Vector2 to)
            => CalcF.Distance(from, to);

        internal static float GetTotalDistance(IEnumerable<Vector2> points)
        {
            float dist = 0.0f;

            if (points?.Count() == 0)
                return dist;

            Vector2 previous = points.First();

            foreach(Vector2 next in points)
            {
                dist += GetDistanceBetween(previous, next);
                previous = next;
            }

            return dist;
        }

        internal static Vector2 GetClosestPoint(Vector2 position, IEnumerable<Vector2> points)
            => points?.Count() == 0 ? position : points.ElementAt(ClosestPointAt(position, points));

        internal static int ClosestPointAt(Vector2 position, IEnumerable<Vector2> points)
        {
            if (points?.Count() == 0)
                return -1;

            float dist = 0.0f;
            int index = 0;
            int i = 0;

            foreach(Vector2 point in points)
            {
                float amount = GetDistanceBetween(position, point);

                if (amount == 0)
                    return i;

                else if (amount < dist)
                {
                    dist = amount;
                    index = i;
                }

                i++;
            }

            return index;
        }

        internal static float GetDistanceTravelled(Vector2 from, Vector2 to, float progress)
            => GetDistanceTravelled(GetDistanceBetween(from, to), progress);

        internal static float GetDistanceTravelled(IEnumerable<Vector2> points, float progress)
            => GetDistanceTravelled(GetTotalDistance(points), progress);

        internal static float GetDistanceRemaining(Vector2 from, Vector2 to, float progress)
            => GetDistanceRemaining(GetDistanceBetween(from, to), progress);

        internal static float GetDistanceRemaining(float dist, float progress)
            => dist - GetDistanceTravelled(dist, progress);

        internal static float GetDistanceTravelled(float dist, float progress)
            => RangeF.Convert(0.0f, 1.0f, 0.0f, dist, progress);

        internal static float GetLength(float from, float to, float progress)
            => RangeF.Convert(0.0f, 1.0f, from, to, progress);

        public static Vector2 GetPosition(Vector2 from, Vector2 to, float progress)
        {
            float x = GetLength(from.X, to.X, progress);
            float y = GetLength(from.Y, to.Y, progress);

            return new Vector2(x, y);
        }

        public static Vector2 GetPosition(IEnumerable<Vector2> points, float progress)
        {
            float travelled = GetDistanceTravelled(points, progress);

            float dist = 0.0f;
            Vector2 previous = points.First();
            int i = 0;

            foreach (Vector2 next in points)
            {
                dist += GetDistanceBetween(previous, next);
                previous = next;

                if (dist >= travelled)
                    break;

                i++;
            }

            float travelProgress = GetTravelProgress(dist, travelled, GetDistanceTo(points, i - 1));
            return GetPosition(points.ElementAt(i - 1), points.ElementAt(i), travelProgress);
        }

        internal static float GetTravelProgress(float dist, float travelled, float lastDist = 0.0f)
            => RangeF.Convert(lastDist, dist, 0.0f, 1.0f, travelled);

        // this adds up all of the distance up to the specified index.
        internal static float GetDistanceTo(IEnumerable<Vector2> points, int index)
        {
            float dist = 0.0f;

            if (points?.Count() == 0)
                return dist;

            Vector2 previous = points.First();
            int i = 0;
            foreach (Vector2 next in points)
            {
                if (index == i)
                    break;

                dist += GetDistanceBetween(previous, next);
                previous = next;
                i++;
            }

            return dist;
        }
    }
}
