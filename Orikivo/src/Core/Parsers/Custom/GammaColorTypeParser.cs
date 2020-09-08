using System;

namespace Orikivo
{
    public class GammaColorTypeParser : CustomTypeParser
    {
        public override bool TryParse(string input, out object result)
        {
            bool isHex = false;
            int hexLen = 0;

            foreach (char c in input)
            {
                if (isHex)
                {

                }


                if (c == '#')
                {
                    isHex = true;
                }
            }

            throw new NotImplementedException();
        }
    }
}
