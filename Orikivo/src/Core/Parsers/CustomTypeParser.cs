using System;

#pragma warning disable CS1998

namespace Orikivo
{
    /// <summary>
    /// Represents an abstract parser for custom <see cref="Type"/>.
    /// </summary>
    public abstract class CustomTypeParser
    {
        public abstract bool TryParse(string input, out object result);
    }

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
