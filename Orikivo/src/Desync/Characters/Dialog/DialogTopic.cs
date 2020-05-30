namespace Orikivo.Desync
{
    public class DialogTopic
    {
        // the general summary of this topic
        public TopicType Type;

        // a direct pointer to what the topic is
        // if none is set, it refers to its generic counterpart
        public string Id;
    }
}
