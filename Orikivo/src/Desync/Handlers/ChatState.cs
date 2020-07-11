namespace Orikivo
{
    public enum ChatState
    {
        Entry, // when you first start talking to an npc
        Speak, // when you are currently speaking to an npc
        Trade, // when you are willing to trade with an npc
        Gift, // when you are gifting to an npc
        Request, // when you are getting a request from an npc
        Give // when you are giving to an npc
    }
}
