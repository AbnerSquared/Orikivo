namespace Orikivo
{
    // rework the dice classes
    public class Dice
    {
        public Dice(int faceCount)
        {
            FaceCount = faceCount;
            Length = _length;
        }

        private static int _length = 20;
        public int Weight { get; } // how heavy the dice is. this impacts the roll.
        internal int Length { get; } // this is used to alter the amount how much is rolled to get a value.
        public int FaceCount { get; set; } // the amount of faces on the dice
    }
}
