using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo
{
    public class GameClient
    {
        private GameEventHandler _events;
        private BaseSocketClient _client;
        private GameLobby _lobby; // contains the root info.
        private bool _active = false;
        public GameClient(Game game, BaseSocketClient client, GameEventHandler events)
        {
            _events = events;
            _lobby = game.Lobby;
            GameProperties properties = GameProperties.Create(game.Mode, game.Users);
            EntryTask = properties.EntryTask;
            Tasks = properties.Tasks;
            ExitTask = properties.ExitTask;
            Data = properties.BaseData;
        }

        // the client's own data that is passed along each game.
        private GameData Data { get; }

        private CancellationTokenSource GameToken { get; } = new CancellationTokenSource();

        private GameTask EntryTask { get; }
        private GameTask ExitTask { get; }
        private List<GameTask> Tasks { get; }

        private GameTask CurrentTask { get; set; }

        internal async Task<GameResult> StartAsync()
        {
            try
            {
                _active = true;
                CurrentTask = EntryTask;
                do
                {
                    GameRoute route = await CurrentTask.StartAsync(_client, _lobby, _events, Data, GameToken.Token);

                    if (!Checks.NotNull(route.TaskId))
                    {
                        if (CurrentTask.Id == ExitTask.Id)
                            _active = false;
                        else
                            CurrentTask = ExitTask; // polish the exit mechanic
                    }
                    else
                    {
                        if (!Tasks.Any(x => x.Id == route.TaskId))
                            throw new Exception("A route attempted to go to a task that doesn't exist.");
                        CurrentTask = Tasks.First(x => x.Id == route.TaskId);
                    }
                } while (_active);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await StopAsync("An exception has occured.", TimeSpan.FromSeconds(3));
                return GameResult.FromException(ex);
            }

            return GameResult.Empty;
        }

        internal async Task StopAsync(string reason = null, TimeSpan? delay = null)
        {
            GameToken.Cancel();
            // catch all client msg handles.
        }
    }
}
