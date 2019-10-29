using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Base rulesets that define how a game is handled.
    /// </summary>
    public class GameBuilder
    {
        private GameBuilder() {}
        // learn about what is required for the game properties to generate.
        // you need the list of users in order to properly determine what attributes need to exist
        public static GameBuilder Create(GameMode mode, List<User> users)
        {
            GameBuilder builder = new GameBuilder();
            List<GameAttribute> attributes = new List<GameAttribute>();

            string task1 = "task:call"; // first task name
            string task2 = "task:read"; // second task name
            string task3 = "task:cancel"; // on cancel
            string task4 = "task:timeout"; // on timeout from cancel.

            // lets create a demo game property.
            string timesCalledAtt = "att:times_called"; // used to check how many times this was called.

            GameAttribute timesCalled = new GameAttribute(timesCalledAtt);

            TaskQueuePacket onCrit1 = new TaskQueuePacket(TaskQueueReason.Success, task2);
            TaskCriterion crit1 = new TaskCriterion(new List<AttributeCriterion> { new AttributeCriterion(timesCalled.Id, 3) }, onCrit1);

            GameUpdatePacket onCall = new GameUpdatePacket(new List<AttributeUpdatePacket> { new AttributeUpdatePacket(timesCalledAtt, 1) });
            GameTrigger crit1Call = new GameTrigger("call", onParseSuccess: new List<GameUpdatePacket>() { onCall });

            TaskQueuePacket onCancel = new TaskQueuePacket(TaskQueueReason.Cancel, task3);

            GameTimer task3Timer = new GameTimer(TimeSpan.FromSeconds(10), new TaskQueuePacket(TaskQueueReason.Timeout, task4));

            GameTask Task1 = new GameTask(task1,
                new List<GameAttribute> { timesCalled },
                new List<GameTrigger> { crit1Call },
                new List<TaskCriterion> { crit1 },
                onCancel); // new GameTimer(TimeSpan.FromSeconds(10), new GameRoute(TaskRoute.Timeout, task4))

            GameTask Task2 = new GameTask(task2,
                new List<GameAttribute>(),
                new List<GameTrigger>(),
                new List<TaskCriterion>(),
                new TaskQueuePacket(TaskQueueReason.Cancel, task4),
                new GameTimer(TimeSpan.FromSeconds(10), new TaskQueuePacket(TaskQueueReason.Timeout, task4)));

            GameTask Task3 = new GameTask(task3,
                new List<GameAttribute>(),
                new List<GameTrigger>(),
                new List<TaskCriterion>(),
                new TaskQueuePacket(TaskQueueReason.Cancel, task4),
                new GameTimer(TimeSpan.FromSeconds(10),
                new TaskQueuePacket(TaskQueueReason.Timeout, task4)));
            GameTask Task4 = new GameTask(task4,
                new List<GameAttribute>(), new List<GameTrigger>(),
                new List<TaskCriterion>(), new TaskQueuePacket(TaskQueueReason.Cancel, null),
                new GameTimer(TimeSpan.FromSeconds(5), new TaskQueuePacket(TaskQueueReason.Timeout, null)));

            builder.EntryTask = Task1;
            // expected: a player types 'call' 3 times, which changes the task to task2, which times out to task4.
            builder.Attributes = attributes;
            builder.Tasks = new List<GameTask> { Task2, Task3, Task4 };
            builder.ExitTask = Task4;

            
            return builder;
        }

        public string Name { get; set; }
        public GameLobbyCriteria ToStart { get; set; }
        public List<GameAttribute> GlobalAttributes { get; set; }
        public List<GameAttribute> UserAttributes { get; set; }
        //public List<GameTaskProperties> Tasks { get; set; }
        public List<GameTrigger> Commands { get; set; } // GameTrigger => GameCommand
        // List of a list of attributes, with what to do on success.
        public List<GameCriterion> ToComplete { get; set; }
        public List<GameRule> Rules { get; set; }
        public GameWindowProperties WindowProperties { get; set; }
        // public GameTaskProperties EntryTask { get; set; }

        public GameTask EntryTask { get; private set; }
        public List<GameAttribute> Attributes { get; private set; } // the root list of attributes.
        public List<GameTask> Tasks { get; private set; } // the list of tasks.



        public GameTask ExitTask { get; private set; }

        public GameData BaseData { get; private set; }
    }

    // create a function determining what to do upon starting from a specific task.
}
