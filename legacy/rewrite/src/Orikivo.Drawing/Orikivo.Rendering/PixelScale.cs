namespace Orikivo
{
    public static class PixelScale
    {
        public static int min = 1;
        public static int max = 4;

        public static int Set(this int x)
        {
            x = (x - 1).InRange(0, int.MaxValue);
            int r = min;
            for(int i = 0; i < x; i++)
            {
                r = Next(r);
                r.Debug();
                if (r > max) break;
            }
            return Ensure(r);
        }     

        public static int Last(this int i)
            => (i / 2);
        
        public static int Next(this int i)
            => (i * 2);

        public static int Ensure(this int i)
            => i.InRange(min, max);
    }
}