namespace Orikivo
{
    public class Trigger
    {
        public string Name { get; set; }
        public string[] Args { get; set; }
        public int ArgCount => Args.Length;

        public bool IsSuccess { get; set; }
    }
}
