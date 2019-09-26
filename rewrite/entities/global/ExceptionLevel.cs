using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public enum ExceptionLevel
    {
        Quiet = 1, // doesn't send anything upon a command incorrectly working
        Default = 2, // includes unknown command errors
        Critical = 3 // only incorporates 'Oops! An error has occured.' errors; actual exceptions
    }
}
