using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Text
{
    internal class EngineExample
    {
        internal void Demo()
        {
            using (var engine = new AsciiEngine(20, 20))
            {
                engine.CurrentGrid.CreateAndAddObject("DVD", '\n', 0, 0, 0,
                    GridCollideMethod.Reflect, new AsciiVector(1, 0, 0, 0));
                string[] frames = engine.GetFrames(0, 9, 1);
                Console.WriteLine(string.Join("\n", frames.Select((x, i) => $"Frame {i}:\n{x}")));
            }
        }
    }
}
