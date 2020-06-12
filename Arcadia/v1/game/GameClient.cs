using Discord.WebSocket;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arcadia.Old
{
    // Handles the actual process of the game.
    public class GameClient
    {
        private GameEventHandler _events;
        private BaseSocketClient _client;
        private GameLobby _lobby; // contains the root info.
        private GameDisplay _display;
        private bool _active = false;
        public GameClient(Game game, BaseSocketClient client, GameEventHandler events)
        {
            _client = client;
            _events = events;
            _lobby = game.Lobby;
            _display = game.Display;
            Id = game.Id;
            GameBuilder properties = GameBuilder.Create(game.Mode, game.Users);
            EntryTask = properties.EntryTask;
            Tasks = properties.Tasks;
            ExitTask = properties.ExitTask;
            Data = properties.BaseData;
            Console.WriteLine($"-- A game client was built. (#{Id}) --");
        }

        // the Id of the game client, deriving from its game.
        public string Id { get; }
        // the client's own data that is passed along each game.
        private GameClientData Data { get; }

        private CancellationTokenSource GameToken { get; } = new CancellationTokenSource();

        private GameTask EntryTask { get; }
        private GameTask ExitTask { get; }
        private List<GameTask> Tasks { get; }

        private GameTask CurrentTask { get; set; }

        // Officially starts the game client, launching at entry task and continuing until an end has been met.
        internal async Task<GameResult> StartAsync()
        {
            try
            {
                _active = true;
                CurrentTask = EntryTask;
                do
                {
                    Console.WriteLine($"-- A task has started execution. ({CurrentTask.Id}) --");
                    TaskQueuePacket route = await CurrentTask.StartAsync(_client, _lobby, _display, Data, GameToken.Token).ConfigureAwait(false);
                    Console.WriteLine($"-- The current task has completed. ({CurrentTask.Id}) --");

                    if (!Check.NotNull(route.NextTaskId))
                    {
                        if (CurrentTask.Id == ExitTask?.Id)
                            _active = false;
                        else
                            CurrentTask = ExitTask; // polish the exit mechanic
                    }
                    else
                    {
                        if (!Tasks.Any(x => x.Id == route.NextTaskId))
                            throw new Exception("A route attempted to go to a task that doesn't exist.");
                        CurrentTask = Tasks.First(x => x.Id == route.NextTaskId);
                        Console.WriteLine($"-- The current task has been updated. ({route.TaskId} => {route.NextTaskId ?? "null"}) --");
                    }
                } while (_active);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await StopAsync("An exception has occured.", TimeSpan.FromSeconds(3));
                return GameResult.FromException(ex);
            }

            Console.WriteLine($"-- The game has completed all tasks. --");
            return GameResult.Empty;
        }

        // Completely halts all actions and closes the client.
        internal async Task StopAsync(string reason = null, TimeSpan? delay = null)
        {
            GameToken.Cancel();
            // catch all client msg handles.
        }
    }
}
