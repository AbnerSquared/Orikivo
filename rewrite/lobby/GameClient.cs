using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // double check with julian about the order of when abilities were active in werewolf
    public class GameClient
    {
        private GameEventHandler _eventHandler;
        private BaseSocketClient _client;
        private GameLobby _lobby;
        // set up the game client with all of their stuff things.
        public GameClient(GameMode mode, BaseSocketClient client, GameLobby lobby, GameEventHandler eventHandler)
        {
            _eventHandler = eventHandler;
            GameProperties props = GameProperties.FromMode(mode);
            Attributes = props.Attributes;
            EntryTask = props.EntryTask; // the first task to start with on the game client.
            Tasks = props.Tasks; // a collection of all other tasks.
        }
        // a list of 
        public List<GameAttribute> Attributes { get; } // a list of attributes that contain info.

        private GameTask EntryTask { get; }
        public List<GameTask> Tasks { get; } // a list of tasks that is used to control the game.

        // the game's starting point.
        // from this point, you could return a GameResult, which could contain all
        // properties that need to be updated.
        public async Task StartAsync()
        {
            bool active = true;
            GameRoute route = await EntryTask.StartAsync(_client, _lobby, _eventHandler).ConfigureAwait(false);
            if (!Checks.NotNull(route.TaskId)) // if the entry route doesn't go anywhere.
                return;

            // create a loop 
            GameTask task;
            while (active)
            {
                if (!Tasks.Any(x => x.Id == route.TaskId)) // if the route doesn't go anywhere.
                    return;

                task = Tasks.First(x => x.Id == route.TaskId); // get the task corresponding to the route.

                route = await task.StartAsync(_client, _lobby, _eventHandler); // get the route from the task.
                
            }

            // end the game.
        }

        // internal async Task OnMessageReceivedAsync(SocketMessage message);

        // this is used to start the game entirely.
        // internal async Task StartAsync();

        // tasks
        
        // init

        // compile [att.forceComplete] // this task is used to let everyone playing know their role.
        // example [att.forceComplete] // this task is used to help new users understand how to play.
        
        // .loop
        
        // new_round [att.forceComplete]
        // this task is used to show what happened during the night.

        // meet [att.forceComplete]
        // this task is used to allow players to discuss or call someone to trial.
        
        // call [att.anyAgree, att.allDecline]
        // this task is a small timeframe in which is used to see if the call was ensured.
        
        // trial [att.suspectSilent, att.suspectResponded, att.timeout]
        // this task is a small time frame that allows the suspect to defend their place.
        
        // verdict [att.forceDiscuss, att.playersVoted, att.timeout]
        // this task is used to determine the outcome of the suspect on trial.

        // obituary [att.forceComplete]
        // this task is used to notify a death, revealing their role. (notes may be revealed)
        
        // discuss [att.forceComplete]
        // this task is used as a means for everyone to discuss a trial before verdict.
        // it is only activated through one player calling a discussion.

        // action [att.timeout, att.abilitiesUsed]
        // this task is used to allow all players with an ability available at this task
        // to utilize it to the best of their ability.

        // results [att.timeout]
        // this task is used to show the end result of the game that was played.
        

    }
}
