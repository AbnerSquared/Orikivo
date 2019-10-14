using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace Orikivo
{
    // this defines what the game needs before actually launching.
    public class GameProperties
    {
        private GameProperties() {}
        // learn about what is required for the game properties to generate.
        // you need the list of users in order to properly determine what attributes need to exist
        public static GameProperties Create(GameMode mode, List<User> users)
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

            GameTaskQueue onCrit1 = new GameTaskQueue(TaskCloseReason.Success, task2);
            TaskCriterion crit1 = new TaskCriterion(new List<AttributeCriterion> { new AttributeCriterion(timesCalled.Id, 3) }, onCrit1);

            AttributeUpdatePacket onCall = new AttributeUpdatePacket(timesCalledAtt, 1);
            GameTrigger crit1Call = new GameTrigger("call", onSuccess: onCall);

            GameTaskQueue onCancel = new GameTaskQueue(TaskCloseReason.Cancel, task3);

            GameTimer task3Timer = new GameTimer(TimeSpan.FromSeconds(10), new GameTaskQueue(TaskCloseReason.Timeout, task4));


            GameTask Task1 = new GameTask(task1,
                new List<GameAttribute> { timesCalled },
                new List<GameTrigger> { crit1Call },
                new List<TaskCriterion> { crit1 },
                onCancel); // new GameTimer(TimeSpan.FromSeconds(10), new GameRoute(TaskRoute.Timeout, task4))

            GameTask Task2 = new GameTask(task2,
                new List<GameAttribute>(),
                new List<GameTrigger>(),
                new List<TaskCriterion>(),
                new GameTaskQueue(TaskCloseReason.Cancel, task4),
                new GameTimer(TimeSpan.FromSeconds(10), new GameTaskQueue(TaskCloseReason.Timeout, task4)));

            GameTask Task3 = new GameTask(task3,
                new List<GameAttribute>(),
                new List<GameTrigger>(),
                new List<TaskCriterion>(),
                new GameTaskQueue(TaskCloseReason.Cancel, task4),
                new GameTimer(TimeSpan.FromSeconds(10),
                new GameTaskQueue(TaskCloseReason.Timeout, task4)));
            GameTask Task4 = new GameTask(task4,
                new List<GameAttribute>(), new List<GameTrigger>(),
                new List<TaskCriterion>(), new GameTaskQueue(TaskCloseReason.Cancel, null),
                new GameTimer(TimeSpan.FromSeconds(5), new GameTaskQueue(TaskCloseReason.Timeout, null)));

            properties.EntryTask = Task1; // expected: a player types 'call' 3 times, which changes the task to task2, which times out to task4.
            properties.Attributes = attributes;
            properties.Tasks = new List<GameTask> { Task2, Task3, Task4 };
            properties.ExitTask = Task4;

            return properties;
        }

        public GameTask EntryTask { get; private set; }
        public List<GameAttribute> Attributes { get; private set; } // the root list of attributes.
        public List<GameTask> Tasks { get; private set; } // the list of tasks.

        public GameTask ExitTask { get; private set; }

        public GameData BaseData { get; private set; }
    }

    // create a function determining what to do upon starting from a specific task.
}
