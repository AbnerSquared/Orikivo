using System;
using System.Collections.Generic;

namespace Arcadia.Multiplayer
{
    public class GameResult
    {
        public static readonly int DailyGameCap = 10;

        public string GameId { get; set; }

        public Dictionary<ulong, PlayerResult> UserIds { get; set; } = new Dictionary<ulong, PlayerResult>();

        public void Apply(ArcadeContainer container, TimeSpan sessionDuration, string bonusGameId)
        {
            foreach ((ulong userId, PlayerResult result) in UserIds)
            {
                if (!container.Users.TryGet(userId, out ArcadeUser user))
                    continue;

                if (result.Money > 0)
                    user.Give(result.Money);

                // NOTE: No exp yet, formulas not implemented
                foreach (StatUpdatePacket packet in result.Stats)
                    packet.Apply(user);

                foreach (ItemUpdatePacket packet in result.Items)
                    ItemHelper.GiveItem(user, packet.Id, packet.Amount);

                if (user.GetVar(Stats.Common.MultiplayerLastGamePlayed) == 0)
                    user.SetVar(Stats.Common.MultiplayerLastGamePlayed, DateTime.UtcNow.Ticks);

                var lastGamePlayed = new DateTime(user.GetVar(Stats.Common.MultiplayerLastGamePlayed));
                user.SetVar(Stats.Common.MultiplayerLastGamePlayed, DateTime.UtcNow.Ticks);

                if (CooldownHelper.DaysSince(lastGamePlayed) >= 1)
                    Var.Clear(user, Stats.Common.MultiplayerGamesPlayedDaily);

                user.AddToVar(Stats.Common.MultiplayerGamesPlayed);

                if (result.Exp > 0)
                {
                    // Only update this var if the game provides experience
                    user.AddToVar(Stats.Common.MultiplayerGamesPlayedDaily);

                    long gameCountDaily = user.GetVar(Stats.Common.MultiplayerGamesPlayedDaily);
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
    }
}
