namespace Orikivo.Networking
{
    public enum HttpApplicationType
    {
        JSON = 1
    }

    public static class HttpApplicationTypeExtension
    {
        public static string Read(this HttpApplicationType type)
            => $"application/{type.ToString().ToLower()}";
    }
}
