using System;
using System.Collections.Generic;
using System.Drawing;
using Padding = Orikivo.Drawing.Padding;

namespace Orikivo.Text
{
    internal static class Utils
    {
        public static char[][] CreateObject(string value, char separatorChar, char emptyChar)
        {
            string[] values = value.Split(separatorChar);
            List<char[]> chars = new List<char[]>();
            foreach (string v in values)
            {
                chars.Add(v.ToCharArray());
            }

            return CreateObject(chars.ToArray(), emptyChar);
        }

        public static char[][] CreateObject(char[][] chars, char emptyChar)
        {
            int trueWidth = GetObjectWidth(chars);

            List<char[]> values = new List<char[]>();

            foreach(char[] row in chars)
            {
                if (row.Length != trueWidth)
                {
                    char[] newRow = new char[trueWidth];
                    row.CopyTo(newRow, 0);

                    for (int i = row.Length - 1; i < trueWidth; i++)
                        newRow[i] = emptyChar;

                    values.Add(newRow);
                }
                else
                    values.Add(row);
            }

            return values.ToArray();
        }

        public static int GetObjectWidth(string[] obj)
        {
            List<char[]> chars = new List<char[]>();
            foreach (string s in obj)
            {
                chars.Add(s.ToCharArray());
            }

            return GetObjectWidth(chars.ToArray());
        }

        /// <summary>
        /// Gets the width of an <see cref="AsciiObject"/> specified by its <see cref="AsciiObject.Chars"/>.
        /// </summary>
		public static int GetObjectWidth(char[][] obj) // char[,]
        {
            int length = 0;
			// char[0][0] = a
			foreach(char[] chars in obj) // char[0] = [a, b, c] char[1] = [d, e, f]
            {
                if (chars.Length > length)
                    length = chars.Length;
            }

            return length;
        }

		/// <summary>
        /// Gets the total frame count that should be rendered with a specified range of time and incremental counter.
        /// </summary>
        public static int GetFrameCount(int fromTime, int toTime, float step)
        {
            // to - from / step ??, Math.Floor to catch extraneous values.
            return (int) Math.Floor((toTime - fromTime) / step);
        }
		
        /// <summary>
        /// Gets the length of each whole character within a grid length.
        /// </summary>
		public static float GetGridStep(int gridLength)
        {
            return (1 / gridLength);
        }

        // getting position for velocity
		// v = cps => Char per Second; for every time % 1 == 0, increase position by v.
		// example: 2 seconds passed, x is at 0, velocity is 5, in two seconds, x is 10.

		// getting position for acceleration
		// a = cps^2 => Char per Second Squared; for every time % 1 == 0, increase v by a, increase p by v.
		// example: 2 seconds passed, x is 0, v is 0, a is 2;
		// in one second, v = 2, x = 2, in two seconds, v = 4, x = 6

		public static float GetAverageVelocity(float currentPos, float initialPos, float currentTime, float initialTime = 0f)
        {
            return (currentPos - initialPos) / (currentTime - initialTime);
        }

		// gets a velocity with an applied acceleration after some time.
		public static float GetVelocity(float initialVelocity, float acceleration, float time)
        {
            return initialVelocity + (acceleration * time);
        }

		// s = s0 + vt
		// gets the new position with a specified initial position, initial velocity, and time.
        /// <summary>
        /// Gets the new precise position retrieved by its initial position, velocity, and the amount of time that has passed.
        /// </summary>
		public static float GetRawDist(int initialPos, float initialVelocity, float time)
        {
            return initialPos + (initialVelocity * time); // initial position can be set by initial time.
        }

        /// <summary>
        /// Gets the new precise position retrieved by its initial position, velocity, acceleration, and the amount of time that has passed.
        /// </summary>
		public static float GetRawDist(int initialPos, float initialVelocity, float acceleration, float time)
        {
			// s = s0 + v0t + 1/2(a(t)^2)
            return initialPos + (initialVelocity * time) + ((1 / 2) * (acceleration * ((float)Math.Pow(time, 2))));
        }

        /// <summary>
        /// Gets the raw point of a value defined by its initial point and vector after a specified amount of time.
        /// </summary>
        public static PointF GetRawPos(int x, int y, AsciiVector vector, float time)
        {
            return new PointF(GetRawDist(x, vector.VX, vector.AX, time), GetRawDist(y, vector.VY, vector.AY, time));
        }

        public static float GetCollisionMultiplier(float rawLength, int gridLength, GridCollideMethod collideMethod)
        {
            return collideMethod switch
            {
                GridCollideMethod.Ignore => 1,
                GridCollideMethod.Reflect => rawLength >= gridLength ? -1 : 1,
                GridCollideMethod.Scroll => 1,
                GridCollideMethod.Stop => rawLength >= gridLength ? 0 : 1,
                _ => 1
            };
        }

        /// <summary>
        /// Gets the true position for a grid based off of its raw position, grid length, grid collision method, and grid padding.
        /// </summary>
		public static Point GetPos(AsciiObject obj, int gridWidth, int gridHeight,
            GridCollideMethod collision, Padding gridPadding, float time)
        {
            float vX = GetVelocity(obj.Vector.VX, obj.Vector.AX, time); // gets their actual velocities
            float vY = GetVelocity(obj.Vector.VY, obj.Vector.AY, time); // gets their actual velocities

            float pX = GetRawDist(obj.X, vX, time); // gets their actual positions after some time
            float pY = GetRawDist(obj.Y, vY, time); // gets their actual positions after some time.

            float pX_MAX = pX + obj.Width;
            float pY_MAX = pY + obj.Height;

            int dX = pX < 0 ? -1 : 1; // directions
            int dY = pX < 0 ? -1 : 1; // directions

            bool canScrollX = collision == GridCollideMethod.Scroll && (pX >= gridWidth || pX < 0);
            bool canScrollY = collision == GridCollideMethod.Scroll && (pY >= gridHeight || pY < 0);
            int gX = (int)Math.Floor((pX - (canScrollX ? gridWidth * dX : 0)));
            int gY = (int)Math.Floor((pY - (canScrollY ? gridHeight * dY : 0)));

            Console.WriteLine($"Velocity.X: {vX}\nVelocity.Y: {vY}\nPosition.X: {pX}\nPosition.Y: {pY}");

            // check if the new points are greater than the grid
            // 3/32 34/32

            // REMOVE:
            int x = (int)Math.Floor(pX_MAX > gridWidth ? gridWidth - obj.Width : pX < 0 ? 0 : (pX));
            int y = (int)Math.Floor(pY_MAX > gridHeight ? gridHeight - obj.Height : pY < 0 ? 0 : (pY));

            return new Point(x, y); // TODO: make safe.
        }

        public static List<CharValue> GetPoints(AsciiObject obj, AsciiGrid grid, float time)
        {
            float vX = GetVelocity(obj.Vector.VX, obj.Vector.AX, time);
            float vY = GetVelocity(obj.Vector.VY, obj.Vector.AY, time);

            int pX = (int)Math.Floor(GetRawDist(obj.X, vX, time));
            int pY = (int)Math.Floor(GetRawDist(obj.Y, vY, time));

            switch(obj.Collider.GridCollider)
            {
                case GridCollideMethod.Ignore:
                    return GetIgnorePoints(pX, pY, obj, grid);

                case GridCollideMethod.Reflect:
                    if (time % 1 != 0)
                        throw new Exception("In order to simulate reflection, time must be a whole number.");
                    return GetReflectPoints(pX, pY, obj, grid); // simulated... try to find a work around be getting a function to get the point regardless

                case GridCollideMethod.Scroll:
                    return GetScrollPoints(pX, pY, obj, grid);

                case GridCollideMethod.Stop:
                    return GetStopPoints(pX, pY, obj, grid);

                default:
                    return GetIgnorePoints(pX, pY, obj, grid);
            }
        }

        public static List<CharValue> GetScrollPoints(int pX, int pY, AsciiObject obj, AsciiGrid grid)
        {
            //float vX = GetVelocity(obj.Vector.VX, obj.Vector.AX, time);
            //float vY = GetVelocity(obj.Vector.VY, obj.Vector.AY, time);

            //int pX = (int)Math.Floor(GetRawDist(obj.X, vX, time));
            //int pY = (int)Math.Floor(GetRawDist(obj.Y, vY, time));

            List<CharValue> values = new List<CharValue>();
            // for now, just work as if it was scrolling.

            // SCROLLING
            for (int y = pY; y < pY + obj.Height; y++)
            {
                int overlapY = (int)Math.Floor((double)(y / grid.Height));
                int cY = y - (overlapY * grid.Height);

                for (int x = pX; x < pX + obj.Width; x++)
                {
                    int overlapX = (int)Math.Floor((double)(x / grid.Width));
                    int cX = x - (overlapX * grid.Width);

                    Console.WriteLine($"({cX}, {cY})");
                    values.Add(new CharValue(obj.Chars[y - pY][x - pX], cX, cY));
                }
            }

            return values;
        }

        public static List<CharValue> GetIgnorePoints(int pX, int pY, AsciiObject obj, AsciiGrid grid)
        {
            List<CharValue> values = new List<CharValue>();

            for (int y = pY; y < pY + obj.Height; y++)
            {
                if (y >= grid.Height)
                    continue;

                for (int x = pX; x < pX + obj.Width; x++)
                {
                    if (x >= grid.Width)
                        continue;

                    values.Add(new CharValue(obj.Chars[y - pY][x - pX], x, y));
                }
            }

            return values;
        }

        public static List<CharValue> GetStopPoints(int pX, int pY, AsciiObject obj, AsciiGrid grid)
        {
            List<CharValue> values = new List<CharValue>();

            for (int y = pY; y < pY + obj.Height; y++)
            {
                int sY = y;
                if (y + (obj.Height - 1) >= grid.Height)
                    sY = grid.Height - ((y - pY) + 1);

                for (int x = pX; x < pX + obj.Width; x++)
                {
                    int sX = x;
                    if (x + (obj.Width - 1) >= grid.Width)
                        sX = grid.Width - ((x - pX) + 1);

                    values.Add(new CharValue(obj.Chars[y - pY][x - pX], sX, sY));
                }
            }

            return values;
        }

        // only known way to get reflect points....?
        public static List<CharValue> GetReflectPoints(int pX, int pY, AsciiObject obj, AsciiGrid grid)
        {
            List<CharValue> values = new List<CharValue>();

            for (int y = pY; y < pY + obj.Height; y++)
            {
                //int height = (grid.Height - 1) - (y - pY);
                //int overlapY = (int)Math.Abs(Math.Floor((double)(pY / height)));
                //int dirY = (int)(Math.Pow(-1, overlapY) * Math.Sign(obj.Vector.VY));
                //int cY = y - (overlapY * height);
                //int aY = dirY >= 0 ? cY : height - cY;
                int aY = GetReflectValue(pY, obj.Vector.VY, obj.Height, y - pY, grid.Height);

                if (aY >= grid.Height || aY < 0)
                    continue;

                for (int x = pX; x < pX + obj.Width; x++)
                {
                    //int width = (grid.Width - 1) - (x - pX);
                    //int overlapX = (int)Math.Abs(Math.Floor((double)(pX / width)));
                    //int dirX = (int)(Math.Pow(-1, overlapX) * Math.Sign(obj.Vector.VX));
                    //int cX = x - (overlapX * width);
                    //int aX = dirX >= 0 ? cX : width - cX;
                    int aX = GetReflectValue(pX, obj.Vector.VX, obj.Width, x - pX, grid.Width);

                    if (aX >= grid.Width || aX < 0)
                        continue;

                    Console.WriteLine($"REF ({aX}, {aY})");

                    values.Add(new CharValue(obj.Chars[y - pY][x - pX], aX, aY));
                }
            }

            return values;
        }

        private static int GetReflectValue(int p, float velocity, int objLength, int objIndex, int gridLength)
        {
            int l = p + objIndex;
            int length = (gridLength - 1) - ((objLength - 1) - objIndex); // (8 - 1) - ((3 - 1) - 0) = 7 - 2 = 5
            int overlap = (int)Math.Abs(Math.Floor((double)(p / length)));
            int dir = (int)Math.Pow(-1, overlap) * Math.Sign(velocity);
            int c = l - (overlap * length);
            int x = dir >= 0 ? c : length - c;
            Console.WriteLine($"[0 => {x} => {length}]");
            return x;
        }

        // DrySimulate(int fromTime, int toTime) // simulates for a range of time.

        // SimulateAsync() // simulates until the task is cancelled or closed.

        // Ignore, Reflect, Scroll, Stop
        // Ignore = do nothing, leave points as is ( x = Math.Floor(GetRawDist()) // check in rendering
        // Reflect = make impact velocity negative ( ((GetRawDist() + obj.Width) - Math.Floor((GetRawDist() + obj.Width) / grid.Width))
        // Scroll = leave velocity as is, set characters outside of the grid bounds to the start of the bounds it hit
        // Stop = halt all velocity in the direction it collided in. GetRawDist() + obj.Width > grid.Width : x = grid.Width - obj.Width - 1

        public static float GetAngledVectorX(float x, float angle)
            => (float)(x * Math.Cos(angle % 360));

        public static float GetAngledVectorY(float y, float angle)
            => (float)(y * Math.Sin(angle % 360));
    }
}
