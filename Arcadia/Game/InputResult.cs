namespace Arcadia

{
    public class InputResult
    {
        public bool IsSuccess { get; set; }

        // the input that was successfully parsed
        public IInput Input { get; set; }
    }
}
