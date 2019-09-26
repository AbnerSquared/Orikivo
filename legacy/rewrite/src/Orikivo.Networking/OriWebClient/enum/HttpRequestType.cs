namespace Orikivo.Networking
{
    public enum HttpRequestType
    {
        GET = 1,
        POST = 2,
        PUT = 3,
        DELETE = 4,
        PATCH = 5,
        TRACE = 6
    }

    public static class HttpRequestTypeExtension
    {
        public static string Read(this HttpRequestType type)
            => type.ToString().ToUpper();
    }
}
