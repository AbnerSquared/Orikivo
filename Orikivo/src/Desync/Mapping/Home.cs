using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class Home : Construct
    {
        public int Tier { get; set; }

        public List<Decor> Decors { get; set; }
        public Storage Storage { get; set; }
    }
}
