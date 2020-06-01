namespace Arcadia
{
    public class ComponentFormatter
    {
        // when this formatter is set on a component, BaseFormatter is the only thing that is used

        public string BaseFormatter { get; set; }
        public string Separator { get; set; }
        public string ElementFormatter { get; set; }
    }
}
