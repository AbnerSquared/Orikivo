namespace Orikivo
{
    public class GameSuccessRoute : GameRoute
    {
        public GameSuccessRoute(string successId, string id) : base(TaskRoute.Success, id)
        {
            SuccessId = successId;
        }

        public string SuccessId { get; }
    }
}
