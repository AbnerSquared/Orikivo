using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Orikivo
{
    public class ProcessManager
    {
        // Used for executing Processes, such as ffmpeg.
        public static Process Build(string path, string args)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = path,
                Arguments = args
            });
        }
    }
}