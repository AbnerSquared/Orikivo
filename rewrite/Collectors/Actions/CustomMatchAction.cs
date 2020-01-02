using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orikivo
{
    public class CustomMatchAction : MatchAction
    {
        public CustomMatchAction(OriCommandContext context)
        {
            Context = context;
        }

        public OriCommandContext Context { get; }
        
        public string Answer { get; set; }
        public int MaxAttempts { get; set; }
        public int CurrentAttempts { get; set; }

        public override async Task OnStartAsync()
        {
            await Context.Channel.SendMessageAsync("What is 2+2?");
        }

        public override async Task<ActionResult> InvokeAsync(SocketMessage message)
        {
            if (message.Content == Answer)
            {
                await Context.Channel.SendMessageAsync("You have guessed correctly.");
                return ActionResult.Success; // marks the filter as a success, and closes it.
            }
            else if (message.Content == "quit")
            {
                await Context.Channel.SendMessageAsync($"You wuss. It was {Answer}.");
                return ActionResult.Fail; // closes the filter
            }

            CurrentAttempts++;

            if (CurrentAttempts == MaxAttempts)
            {
                await Context.Channel.SendMessageAsync($"You have ran out of attempts. The answer was {Answer}.");
                return ActionResult.Fail; // closes the filter
            }

            await Context.Channel.SendMessageAsync($"Incorrect. You have {MaxAttempts - CurrentAttempts} left.");
            return ActionResult.Continue; // lets the filter keep going
        }

        public override async Task OnTimeoutAsync(SocketMessage message)
        {
            await Context.Channel.SendMessageAsync($"You ran out of time. The answer was {Answer}.");
        }
    }
}
