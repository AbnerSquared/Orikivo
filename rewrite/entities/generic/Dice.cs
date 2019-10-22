using System;

namespace Orikivo
{
    // rework the dice classes
    public class Dice : IEquatable<Dice>
    {
        private const int RAND_LENGTH = 20; // used to help randomize sides.
        public Dice(int sides)
        {
            Sides = sides;
            Length = RAND_LENGTH;
        }

        public static Dice Default => new Dice(6);


        internal int Length { get; } // this is used to alter the amount how much is rolled to get a value.
        public int Sides { get; set; } // the amount of faces on the dice
        public bool Equals(Dice dice)
            => Sides == dice.Sides;

        public static bool operator ==(Dice d1, Dice d2)
        {
            return d1.Sides == d2.Sides;
        }

        public static bool operator !=(Dice d1, Dice d2)
        {
            return d1.Sides != d2.Sides;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Dice))
                return Equals(obj as Dice);
            return false;
        }
        public override int GetHashCode()
            => Sides * Length;
    }
}
