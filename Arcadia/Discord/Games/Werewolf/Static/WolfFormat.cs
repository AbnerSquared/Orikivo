using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Multiplayer.Games.Werewolf;
using Orikivo;
using Orikivo.Text;

namespace Arcadia.Multiplayer.Games
{
    public static class WolfFormat
    {
        public static readonly List<string> DefaultRandomNames = new List<string>
        {
            "Tom", "Rachel", "Jerry", "Nathan", "Richard", "Joshua", "Nicholas", "Julian", "Abigail"
        };

        public static string WriteAbilityHeader(GameSession session)
        {
            return session.ValueOf<WolfAbility>(WolfVars.CurrentAbility) switch
            {
                WolfAbility.Peek => WritePeekHeader(),
                WolfAbility.Protect => WriteProtectHeader(),
                WolfAbility.Feast => WriteFeastHeader(),
                WolfAbility.Hunt => WriteHuntHeader(),
                _ => "UNKNOWN_ABILITY_HEADER"
            };
        }

        public static string WriteAbilityList(PlayerData invoker, GameSession session)
        {
            return session.ValueOf<WolfAbility>(WolfVars.CurrentAbility) switch
            {
                WolfAbility.Peek => WritePeekList(invoker, session),
                WolfAbility.Feast => WriteFeastList(invoker, session),
                _ => "UNKNOWN_ABILITY_LIST"
            };
        }

        public static string WriteAbilityResult(PlayerData target, GameSession session)
        {
            return session.ValueOf<WolfAbility>(WolfVars.CurrentAbility) switch
            {
                WolfAbility.Peek => WritePeekResult(target),
                WolfAbility.Feast => WriteFeastResult(target),
                _ => "UNKNOWN_ABILITY_RESULT"
            };
        }

        public static string WriteFeastResult(PlayerData target)
            => $"**{target.ValueOf<string>(WolfVars.Name)}** has been chosen for tonight's feast.";

        public static string WriteProtectHeader()
            => "🛡️ Choose a player to protect:";

        public static string WriteFeastHeader()
            => "🍖 Choose a player to feast on:";

        public static string WritePeekHeader()
            => "🔍 Choose a player to inspect:";

        public static string WriteHuntHeader()
            => "🔫 You have been chosen for tonight's feast. Choose someone to kill before this occurs!";

        public static void WritePlayerList(GameSession session, GameServer server)
        {
            DisplayContent main = server.GetBroadcast(WolfChannel.Main).Content;

            // Initialize all of the player slots
            foreach (PlayerData player in session.Players)
                main.GetGroup("players").Set(player.ValueOf<int>(WolfVars.Index), WritePlayerInfo(player, session));

            main.GetComponent("players").Draw();
        }

        public static string WriteDeathText(WolfDeathMethod method)
        {
            return method switch
            {
                WolfDeathMethod.Hunted => "While sleeping sound, the echoes of a rifle pierced the nightly atmosphere, putting an end to their breathing.",
                WolfDeathMethod.Wolf => "They were mauled by werewolves, leaving barely anything to identify them by.",
                WolfDeathMethod.Hang => "They have been left to hang from the suspicion of the village.",
                WolfDeathMethod.Injury => "They have succumbed to their injuries.",
                _ => "They have been eliminated from an unknown source."
            };
        }

        public static string WriteProtectText(PlayerData player)
            => $"{player.ValueOf<string>(WolfVars.Name)} was protected from the dangers that lurked last night.";

        public static string WriteHurtText(PlayerData player)
            => $"{player.ValueOf<string>(WolfVars.Name)} has been injured, but lives to tell the tale.";

        public static string WriteDeathRemainText(GameSession session)
        {
            int remaining = session.Players.Count(x => !x.ValueOf<WolfStatus>(WolfVars.Status).HasFlag(WolfStatus.Dead));
            return $"\n> **{remaining:##,0}** {Format.TryPluralize("resident", remaining)} remain{(remaining == 1 ? "s" : "")}. Tread carefully.";
        }

        public static string WriteAccuseText(PlayerData accuser, PlayerData suspect)
        {
            // Instead of referencing usernames, store it in a property value,
            // just in case Randomize names is enabled.
            return $"{accuser.Source.User.Username} has accused {suspect.Source.User.Username} of being a werewolf. Does anyone else agree?";
        }

        public static string WriteStartText()
            => "A new dawn begins. Your village has been overrun with werewolves, and it is up to you to eliminate the threat before it becomes too much.";

        public static IEnumerable<string> WriteRandomFacts(GameSession session)
        {
            var strings = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                strings.Add(WriteRandomFact(session));
            }

            return strings;
        }

        public static string WriteRandomFact(GameSession session)
        {
            // get random facts??
            int i = RandomProvider.Instance.Next(0, 2);

            return i switch
            {
                // Who was the first to die?
                // Who got the most people killed?
                // How long did this game last?
                _ => "This is a random fact placeholder."
            };
        }

        public static string WriteRandomWinSummary(WolfGroup group)
        {
            return group switch
            {
                WolfGroup.Villager => "The villagers were able to snuff out all of the werewolves, cleansing their village once and for all.",
                WolfGroup.Werewolf => "After a long struggle, the werewolves were able to overthrow the village, providing a feast to last months.",
                _ => throw new Exception("Invalid winning group specified.")
            };
        }

        public static void WriteMainHeader(GameServer server, GameSession session)
        {
            DisplayContent main = server.GetBroadcast(WolfChannel.Main).Content;
            var totalRounds = session.ValueOf<int>(WolfVars.CurrentRound);
            var currentPhase = session.ValueOf<WolfPhase>(WolfVars.CurrentPhase);
            string phaseInfo = WritePhaseHeader(currentPhase);
            main.GetComponent("header").Draw(totalRounds.ToString("##,0"), phaseInfo);
        }

        public static string WritePhaseHeader(WolfPhase phase)
        {
            return phase switch
            {
                WolfPhase.Day => "🌤️ **Day** (**3:00** until **Night**)",
                WolfPhase.Night => "☄️ **Night**",
                _ => throw new Exception("Invalid phase specified")
            };
        }

        public static string WriteProtectResult(PlayerData target)
        {
            return $"You have chosen to keep **{target.ValueOf<string>(WolfVars.Name)}** safe.";
        }

        public static string WriteRoleInfo(WolfPlayer player, GameSession session)
        {
            if (session.GetConfigValue<bool>(WolfConfig.RevealPartners))
                if (player.Role.Ability.HasFlag(WolfAbility.Feast))
                    return WriteAbilityPartners(player, session);

            return $"> You are a **{player.Role.Name}**.";
        }

        public static string WritePeekResult(PlayerData target)
        {
            if (target.ValueOf<WolfRole>(WolfVars.Role).Passive.HasFlag(WolfPassive.Wolfish))
                return $"**{target.ValueOf<string>(WolfVars.Name)}** is a werewolf.";

            return $"**{target.ValueOf<string>(WolfVars.Name)}** is innocent.";
        }

        public static string WritePlayerInfo(WolfPlayer player, GameSession session)
        {
            var info = new StringBuilder();

            // Get public expression icon
            info.Append(WriteExpression(player, session));

            info.Append($" • ");

            if (player.Status.HasFlag(WolfStatus.Revealed))
                info.Append($"[{player.Role.Name}] ");

            if (player.Status.HasFlag(WolfStatus.Dead))
                info.Append($"~~*{player.Name}*~~ (Dead)");
            else
            {
                info.Append($"**{player.Name}**#{player.Position}");

                if (session.ValueOf<ulong>(WolfVars.Suspect) == player.Connection.User.Id)
                {
                    info.Append(" (Suspect)");
                }
                else if (player.Status.HasFlag(WolfStatus.Hurt))
                {
                    info.Append("(Hurt)");
                }
            }

            return info.ToString();
        }


        public static string WriteExpression(WolfPlayer player, GameSession session)
        {
            if (player.Status.HasFlag(WolfStatus.Dead))
                return "💀";

            if (session.ValueOf<ulong>(WolfVars.Suspect) == player.Connection.User.Id)
                return "😟";

            if (player.Status.HasFlag(WolfStatus.Hurt))
                return "🤕";

            return "😐";
        }

        // This writes the list of peeks for a single seer
        public static string WritePeekList(PlayerData peeker, GameSession session)
        {
            // If the specified player is not a peeker, throw an error
            if (!peeker.ValueOf<WolfRole>(WolfVars.Role).Ability.HasFlag(WolfAbility.Peek))
                throw new Exception("Expected a peeker, but is missing peek ability");

            var summary = new StringBuilder();
            var peeks = session.ValueOf<List<WolfPeekData>>(WolfVars.Peeks);

            foreach (WolfPeekData peek in peeks)
            {
                // If the specified player is the same as the peeker, ignore them
                if (peek.UserId == peeker.Source.User.Id)
                    continue;

                PlayerData player = session.DataOf(peek.UserId);

                // If shared peeking is enabled and the player can also peek, ignore them
                if (session.GetConfigValue<bool>(WolfConfig.SharedPeeking))
                {
                    if (player.ValueOf<WolfRole>(WolfVars.Role).Ability.HasFlag(WolfAbility.Peek))
                        continue;
                }

                summary.Append(WritePeekState(player, peek.Innocent, peek.RevealedTo.Contains(peeker.Source.User.Id)));

                if (WerewolfGame.IsAbilityShared(session, WolfAbility.Peek))
                {
                    int voteCount = WerewolfGame.GetAbilityVote(session, player).VoterIds.Count;

                    if (voteCount > 0)
                        summary.Append($" (**{voteCount:##,0}** {Format.TryPluralize("vote", voteCount)})");
                }

                summary.AppendLine();
            }

            return summary.ToString();
        }

        public static string WriteFeastList(PlayerData wolf, GameSession session)
        {
            var summary = new StringBuilder();

            var votes = session.ValueOf<List<WolfVoteData>>(WolfVars.AbilityVotes);

            foreach (WolfVoteData vote in votes)
            {
                // If the specified player is the same as the peeker, ignore them
                if (vote.UserId == wolf.Source.User.Id)
                    continue;

                PlayerData player = session.DataOf(vote.UserId);

                // If the player is dead
                if (player.ValueOf<WolfStatus>(WolfVars.Status).HasFlag(WolfStatus.Dead))
                    continue;

                summary.AppendLine($"• **{player.ValueOf<string>(WolfVars.Name)}**#{player.ValueOf<int>(WolfVars.Index)}");

                if (WerewolfGame.IsAbilityShared(session, WolfAbility.Feast))
                {
                    // If this player has this ability and it's shared
                    if (player.ValueOf<WolfRole>(WolfVars.Role).Ability.HasFlag(WolfAbility.Feast))
                        continue;

                    int voteCount = WerewolfGame.GetAbilityVote(session, player).VoterIds.Count;

                    if (voteCount > 0)
                        summary.Append($" (**{voteCount:##,0}** {Format.TryPluralize("vote", voteCount)})");
                }
            }

            return summary.ToString();
        }

        // This writes a peek state for a single player on the current seer.
        public static string WritePeekState(PlayerData player, bool innocent, bool isVisible = false)
        {
            string state = isVisible ? innocent ? "🐺" : "😇" : "🎭";

            return $"{state} • **{player.ValueOf<string>(WolfVars.Name)}**#{player.ValueOf<int>(WolfVars.Index)}";
        }

        public static string WriteVoteCounter(GameSession session)
        {
            // You don't want to explicitly show that there are villagers with a role, so hide the state of all of the roles beforehand.
            int yes = session.Players.Count(x => session.ValueOf<ulong>(WolfVars.Suspect) != x.Source.User.Id && x.ValueOf<WolfVote>(WolfVars.Vote) == WolfVote.Die);
            int no = session.Players.Count(x => session.ValueOf<ulong>(WolfVars.Suspect) != x.Source.User.Id && x.ValueOf<WolfVote>(WolfVars.Vote) == WolfVote.Live);
            int pending = session.Players.Count(x => session.ValueOf<ulong>(WolfVars.Suspect) != x.Source.User.Id && x.ValueOf<WolfVote>(WolfVars.Vote) == WolfVote.Pending);

            var counter = new StringBuilder();

            // yes
            counter.Append("🔵", yes);

            // no
            counter.Append("🔴", no);

            // pending
            counter.Append("⚪", pending);

            return counter.ToString();
        }

        public static string WriteAbilityPartners(WolfPlayer player, GameSession session)
        {
            var feastInfo = new StringBuilder();

            foreach (WolfAbility ability in player.Role.Ability.GetFlags())
            {
                // Filter out non-feast and self
                IEnumerable<PlayerData> partners = session
                    .Players
                    .Where(x => !x.ValueOf<WolfStatus>(WolfVars.Status).HasFlag(WolfStatus.Dead)
                                && x.ValueOf<WolfRole>(WolfVars.Role).Ability.HasFlag(ability)
                                && x.Source.User.Id != player.Connection.User.Id);

                feastInfo.AppendLine($"> Your **{ability.ToString()}** teammates are:");

                foreach (PlayerData partner in partners)
                    feastInfo.AppendLine($"• **{partner.ValueOf<string>(WolfVars.Name)}**#{partner.ValueOf<int>(WolfVars.Index)}");
            }

            return feastInfo.ToString();
        }
    }
}