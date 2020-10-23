using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arcadia.Multiplayer
{
    public class TriviaPlayerData
    {
        [Property("score")]
        public int Score { get; internal set; }

        [Property("streak")]
        public int Streak { get; internal set; }

        [Property("answer_state")]
        public TriviaAnswerState AnswerState { get; internal set; }

        [Property("answer_position")]
        public int AnswerPosition { get; internal set; }

        public List<GameProperty> GetProperties(string id)
        {
            return
                GetType()
                    .GetProperties()
                    .Where(x => x.GetCustomAttribute<PropertyAttribute>()?.Id == id)
                    .Select(x => GameProperty.Create(x.GetCustomAttribute<PropertyAttribute>()?.Id, x.GetValue(this), true))
                    .ToList();
        }
    }
}