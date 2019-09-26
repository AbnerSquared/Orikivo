using System;

namespace Orikivo
{
    // a random hash used to randomly generate such.
    // allows using a seed to repeat certain things.
    public class Seed
    {
        public Seed(ulong rawSeed)
        {
            // enforces 0 to 1.
            double pureSeed = Math.Abs(Math.Sin(rawSeed));
        }

        // you can enforce a range of 0 to 1 using Abs.Sin

        


    }
}