using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Arcadia.Multiplayer
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GamePropertyAttribute : Attribute
    {
        public GamePropertyAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; internal set; }
    }

    public enum TriviaAnswerState
    {
        Pending = 1,
        Correct = 2,
        Incorrect = 3
    }

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
                .Where(x => x.GetCustomAttribute<GamePropertyAttribute>() != null)
                .Select(x => GameProperty.Create(x.GetCustomAttribute<GamePropertyAttribute>().Id, x.GetValue(this), true))
                .ToList();
        }
    }

    public interface IPlayerData
    {
        Player Source { get; }

        void SetValue(string id, object value);

        void SetValue(string id, string fromId);

        void AddToValue(string id, int value);

        object ValueOf(string id);

        T ValueOf<T>(string id);

        void ResetProperty(string id);

        void Reset();
    }

    /// <summary>
    /// Represents the data of a <see cref="Player"/> from a <see cref="GameSession"/>.
    /// </summary>
    public class PlayerData
    {
        public Player Source { get; internal set; }

        public List<GameProperty> Properties { get; set; }

        public void SetValue(string id, object value)
        {
            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not the specified property '{id}'");

            Properties.First(x => x.Id == id).Set(value);
        }

        // Sets a value from another value already specified
        public void SetValue(string id, string fromId)
        {
            GetProperty(id).Set(GetProperty(fromId).Value);
        }

        public void AddToValue(string id, int value)
        {
            GameProperty property = GetProperty(id);

            if (property.ValueType != typeof(int))
                throw new Exception($"Cannot add to the specified property '{id}' as it is not a type of Int32");

            property.Value = (int)property.Value + value;
        }

        public void ResetProperty(string id)
            => GetProperty(id).Reset();

        public GameProperty GetProperty(string id)
        {
            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not the specified property '{id}'");

            return Properties.First(x => x.Id == id);
        }

        public object ValueOf(string id)
            => GetProperty(id)?.Value;

        public T ValueOf<T>(string id)
        {
            GameProperty property = GetProperty(id);

            if (property.ValueType?.IsEquivalentTo(typeof(T)) ?? false)
                return (T) property.Value;

            throw new Exception($"The specified property '{id}' does not match the implicit type reference of {typeof(T).Name}");
        }

        public void Reset()
        {
            foreach (GameProperty property in Properties)
                property.Reset();
        }

        public override string ToString()
        {
            var info = new StringBuilder();
            info.AppendLine($"Data for {Source.User.Username}:");

            foreach (GameProperty property in Properties)
                info.AppendLine($"{property.Id}: {property.Value.ToString()}");

            return info.ToString();
        }
    }

}
