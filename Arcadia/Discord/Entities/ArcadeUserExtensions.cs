namespace Arcadia
{
    public static class ArcadeUserExtensions
    {
        /*
        public static long GetVar(this ArcadeUser user, string id)
        {
            return Var.GetValue(user, id);
        }

        public static void SetVar(this ArcadeUser user, string id, long value)
        {
            if (value == 0)
            {
                if (user.Stats.ContainsKey(id))
                    user.Stats.Remove(id);

                user.SetQuestProgress(id);
                return;
            }

            if (!user.Stats.TryAdd(id, value))
                user.Stats[id] = value;

            user.SetQuestProgress(id);
        }
        */
    }
}