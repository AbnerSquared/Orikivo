using System.Drawing;
using Orikivo.Utility;

namespace Orikivo
{
    public class CaptchaBuilder
    {
        public CaptchaBuilder()
        {
            Key = KeyBuilder.Generate();
            Captcha = CaptchaEngine.Generate(Key);
        }
        
        public Bitmap Captcha { get; set; } // what the Verifier sends for people to solve.
        public string Key { get; set; } // what the Image actually says.
    }

    public enum Difficulty
    {
        
    }

    public class Captcha
    {
        public Bitmap Capture {get; set;}
        public string Code {get; set; }
        public int Checks {get; set;}
        public Difficulty Difficulty {get; set;}

        public void TrySolve()
        {
            
        }

        public void Refresh()
        {
        }
    }
}