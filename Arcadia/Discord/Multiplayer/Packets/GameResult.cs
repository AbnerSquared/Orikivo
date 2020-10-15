using System;
using System.Collections.Generic;
using System.Linq;
using Orikivo;
using Orikivo.Framework;

namespace Arcadia.Multiplayer
{
    public class GameResult
    {
        public static readonly int DailyGameCap = 10;

        public string GameId { get; internal set; }

        // This also includes specified configuration values
        public IReadOnlyList<GameProperty> SessionProperties { get; internal set; }

        public IReadOnlyDictionary<ulong, PlayerResult> PlayerResults { get; internal set; }

        public TimeSpan SessionDuration { get; internal set; }

        public PlayerResult GetPlayerResult(ulong userId)
        {
            return PlayerResults.FirstOrDefault(u => u.Key == userId).Value;
        }

        public void Apply(ArcadeContainer container, string bonusGameId)
        {
            foreach ((ulong userId, PlayerResult result) in PlayerResults)
            {
                if (!container.Users.TryGet(userId, out ArcadeUser user))
                    continue;

                if (result.Money > 0)
                    user.Give(result.Money);

                foreach (StatUpdatePacket packet in result.Stats)
                    packet.Apply(user);

                foreach (ItemUpdatePacket packet in result.Items)
                    ItemHelper.GiveItem(user, packet.Id, packet.Amount);

                if (user.GetVar(Stats.Multiplayer.LastGamePlayed) == 0)
                    user.SetVar(Stats.Multiplayer.LastGamePlayed, DateTime.UtcNow.Ticks);

                var lastGamePlayed = new DateTime(user.GetVar(Stats.Multiplayer.LastGamePlayed));
                user.SetVar(Stats.Multiplayer.LastGamePlayed, DateTime.UtcNow.Ticks);

                if (CooldownHelper.DaysSince(lastGamePlayed) >= 1)
                    Var.Clear(user, Stats.Multiplayer.GamesPlayedDaily);

                user.AddToVar(Stats.Multiplayer.GamesPlayed);

                if (result.Exp > 0)
                {
                    // Only update this var if the game provides experience
                    user.AddToVar(Stats.Multiplayer.GamesPlayedDaily);

                    long gameCountDaily = user.GetVar(Stats.Multiplayer.GamesPlayedDaily);
                    bool isBonusGame = GameId == bonusGameId;

                    float baseMultiplier = 1f;

                    // grant bonus exp multiplier when playing the bonus game
                    if (isBonusGame)
                    {
                        // do not grant multiplier if the daily game cap has been exceeded
                        if (gameCountDaily < DailyGameCap)
                            baseMultiplier += 0.5f;
                    }

                    // if the daily cap has been exceeded, start providing 40% reduced EXP, regardless of it being a bonus game
                    if (gameCountDaily > DailyGameCap)
                    {
                        baseMultiplier -= 0.4f;
                    }

                    ulong exp = (ulong)Math.Floor(result.Exp * baseMultiplier);
                    user.Exp += exp;
                }

                container.Users.AddOrUpdate(userId, user);
            }
        }

        public GameProperty GetProperty(string id)
        {
            Logger.Debug($"Getting property {id}");

            if (SessionProperties.All(x => x.Id != id))
                throw new ValueNotFoundException("Could not find the specified property", id);

            return SessionProperties.First(x => x.Id == id);
        }

        public object ValueOf(string id)
            => GetProperty(id).Value;

        public bool ValueExists(string id)
            => SessionProperties.Any(x => x.Id == id);

        public T ValueOf<T>(string id)
        {
            GameProperty property = GetProperty(id);

            if (property.ValueType == null || !property.ValueType.IsEquivalentTo(typeof(T)))
                throw new Exception("The specified type within the property does not match the implicit type reference");

            return (T)property.Value;
        }

        public Type TypeOf(string id)
            => GetProperty(id).ValueType;
    }
}
