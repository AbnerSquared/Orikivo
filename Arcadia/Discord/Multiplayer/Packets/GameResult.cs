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
        public static readonly TimeSpan MinGameDurationPay = TimeSpan.FromMinutes(2);

        public static readonly long ReducedGamePay = 5; // what games pay after the daily cap
        public static readonly long BaseGamePay = 25;
        public static readonly long GameBonusPay = 5; // extra orite given if playing a bonus game
        public static readonly long CapGamePay = 50; // If you play 10 games in a single day, you get paid an extra 50 Orite

        public string GameId { get; internal set; }

        // This also includes specified configuration values
        public IReadOnlyList<GameProperty> SessionProperties { get; internal set; }

        public IReadOnlyDictionary<ulong, PlayerResult> Players { get; internal set; }

        public TimeSpan SessionDuration { get; internal set; }

        // If true, DO NOT use the base payout.
        public bool OverrideBasePay { get; internal set; } = false;

        public PlayerResult GetPlayerResult(ulong userId)
        {
            return Players.FirstOrDefault(u => u.Key == userId).Value;
        }

        private float GetMinPayScalar()
        {
            long minGameTicks = SessionDuration.Ticks;
            long gameLengthTicks = MinGameDurationPay.Ticks;

            return 1.0f + ((gameLengthTicks - minGameTicks) / (gameLengthTicks));
        }

        public long GetPayValue(long dailyGameCount, bool isBonusGame)
        {
            long pay = 0;

            long basePay = (dailyGameCount > DailyGameCap) ?
                ReducedGamePay : dailyGameCount == DailyGameCap ?
                CapGamePay : BaseGamePay;

            if (SessionDuration >= MinGameDurationPay)
            {
                if (isBonusGame)
                    pay += GameBonusPay; // only get bonus pay if the min cap is reached

                pay += basePay;
            }
            else
            {
                pay = Math.Max(0, (long)Math.Floor(basePay * GetMinPayScalar()));
            }

            return pay;
        }

        public void Apply(ArcadeContainer container, string bonusGameId)
        {
            if (Players == null)
                return;

            bool isBonusGame = GameId == bonusGameId;

            foreach ((ulong userId, PlayerResult result) in Players)
            {
                if (!container.Users.TryGet(userId, out ArcadeUser user))
                    continue;

                Var.SetIfEmpty(user, Stats.Multiplayer.LastGamePlayed, DateTime.UtcNow.Ticks);
                var lastGamePlayed = new DateTime(user.GetVar(Stats.Multiplayer.LastGamePlayed));
                user.SetVar(Stats.Multiplayer.LastGamePlayed, DateTime.UtcNow.Ticks);

                if (CooldownHelper.DaysSince(lastGamePlayed) >= 1)
                    Var.Clear(user, Stats.Multiplayer.GamesPlayedDaily, Stats.Multiplayer.GamesPaidDaily);

                if (OverrideBasePay && result.Money > 0) // This overrides the default game pay
                    user.Give(result.Money);
                else
                {
                    long money = GetPayValue(user.GetVar(Stats.Multiplayer.GamesPaidDaily), isBonusGame);

                    if (money > 0)
                    {
                        user.AddToVar(Stats.Multiplayer.GamesPaidDaily);
                        user.Give(money);
                    }
                }

                foreach (StatUpdatePacket packet in result.Stats)
                    packet.Apply(user);

                foreach (ItemUpdatePacket packet in result.Items)
                    ItemHelper.GiveItem(user, packet.Id, packet.Amount);

                user.AddToVar(Stats.Multiplayer.GamesPlayed);

                if (result.Exp > 0)
                {
                    // Only update this var if the game provides experience
                    user.AddToVar(Stats.Multiplayer.GamesPlayedDaily);

                    long gameCountDaily = user.GetVar(Stats.Multiplayer.GamesPlayedDaily);
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
