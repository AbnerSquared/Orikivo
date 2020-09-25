using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arcadia.Multiplayer
{
    public class TriviaPlayerData
    {
        [GameProperty("score")]
        public int Score { get; internal set; }

        [GameProperty("streak")]
        public int Streak { get; internal set; }

        [GameProperty("answer_state")]
        public TriviaAnswerState AnswerState { get; internal set; }

        [GameProperty("answer_position")]
        public int AnswerPosition { get; internal set; }

        public List<GameProperty> GetProperties(string id)
        {
            return
                GetType()
                    .GetProperties()
                    .Where(x => x.GetCustomAttribute<GamePropertyAttribute>()?.Id == id)
                    .Select(x => GameProperty.Create(x.GetCustomAttribute<GamePropertyAttribute>()?.Id, x.GetValue(this), true))
                    .ToList();
        }
    }
}