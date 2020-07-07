namespace Orikivo.Desync
{
    // Maybe change the name of this to Expression OR Mood.
    /// <summary>
    /// Represents the mood that a <see cref="Dialogue"/> outputs.
    /// </summary>
    public enum DialogTone
    {
        Neutral = 1,
        Happy = 2,
        Sad = 3,
        Confused = 4,
        Shocked = 5
    }

    public class ToneModifier
    {
        public DialogTone Tone { get; set; }

        // the strength of a specified tone.
        public float Impact { get; set; }
    }

}
