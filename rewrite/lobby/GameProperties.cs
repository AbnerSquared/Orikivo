using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace Orikivo
{
    // this defines what the game needs before actually launching.
    public class GameProperties
    {
        // learn about what is required for the game properties to generate.
        public static GameProperties FromMode(GameMode mode, BaseSocketClient client, GameLobby lobby, GameEventHandler eventHandler)
        {
            GameProperties properties = new GameProperties();
            List<GameAttribute> attributes = new List<GameAttribute>();

            string task1 = "task:call"; // first task name
            string task2 = "task:read"; // second task name
            string task3 = "task:cancel"; // on cancel
            string task4 = "task:timeout"; // on timeout from cancel.

            // lets create a demo game property.
            string timesCalledAtt = "att:times_called"; // used to check how many times this was called.

            GameAttribute timesCalled = new GameAttribute(timesCalledAtt);

            GameRoute onCrit1 = new GameRoute(TaskRoute.Success, task2);
            TaskCriterion crit1 = new TaskCriterion(new List<AttributeCriterion> { new AttributeCriterion(timesCalled, 3) }, onCrit1);

            GameAttributeUpdate onCall = new GameAttributeUpdate(timesCalledAtt, 1);
            GameTrigger crit1Call = new GameTrigger("call", onSuccess: onCall);

            GameRoute onCancel = new GameRoute(TaskRoute.Cancel, task3);

            GameTimer task3Timer = new GameTimer(TimeSpan.FromSeconds(10), new GameRoute(TaskRoute.Timeout, task4));


            GameTask Task1 = new GameTask(client, lobby, eventHandler, task1,
                new List<GameAttribute> { timesCalled },
                new List<GameTrigger> { crit1Call },
                new List<TaskCriterion> { crit1 },
                onCancel); // new GameTimer(TimeSpan.FromSeconds(10), new GameRoute(TaskRoute.Timeout, task4))

            GameTask Task2 = new GameTask(client, lobby, eventHandler, task2,
                new List<GameAttribute>(),
                new List<GameTrigger>(),
                new List<TaskCriterion>(),
                new GameRoute(TaskRoute.Cancel, task4),
                new GameTimer(TimeSpan.FromSeconds(10), new GameRoute(TaskRoute.Timeout, task4)));

            GameTask Task3 = new GameTask(client, lobby, eventHandler, task3,
                new List<GameAttribute>(),
                new List<GameTrigger>(),
                new List<TaskCriterion>(),
                new GameRoute(TaskRoute.Cancel, task4),
                new GameTimer(TimeSpan.FromSeconds(10),
                new GameRoute(TaskRoute.Timeout, task4)));
            GameTask Task4 = new GameTask(client, lobby, eventHandler, task4,
                new List<GameAttribute>(), new List<GameTrigger>(),
                new List<TaskCriterion>(), new GameRoute(TaskRoute.Cancel, null),
                new GameTimer(TimeSpan.FromSeconds(5), new GameRoute(TaskRoute.Timeout, null)));

            properties.EntryTask = Task1; // expected: a player types 'call' 3 times, which changes the task to task2, which times out to task4.
            properties.Attributes = attributes;
            properties.Tasks = new List<GameTask> { Task2, Task3, Task4 };

            return properties;
        }

        public GameTask EntryTask { get; set; }
        public List<GameAttribute> Attributes { get; set; } // the root list of attributes.
        public List<GameTask> Tasks { get; set; } // the list of tasks.
    }

    // create a function determining what to do upon starting from a specific task.
}
