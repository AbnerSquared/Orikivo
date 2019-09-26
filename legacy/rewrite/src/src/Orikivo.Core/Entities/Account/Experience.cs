using System.Collections.Generic;
using Discord.WebSocket;
using System.Linq;
using System;

namespace Orikivo
{
    // This is for both LocalExperience and GlobalExperience
    public interface IData
    {
        int Level { get; }
        double RawLevel { get; }
        ulong Experience { get; }
        double GetLevel(); // To get level from exp.
        double GetRawLevel(); // To get raw level.
        void TryUpdateLevel(); // To auto-update your level in comparison to your experience.
        double LevelPercentile(); // The percentage of completion on your level.
        double ExperienceOnLevel(double l); // The exp needed to be at a level.
        double ExperienceBetweenLevels(double a, double b); // exp needed in between two levels.
        double RemainderToLevel(double l); // remaining exp needed to a specified level.
    }

    // make a for statement for constantly checking how much you levelled up

    // Build a different formula for active messaging, for say maybe ???
    // A class for saving how active a user is in a server.
    public class ActiveData : IData
    {
        public int Level { get; set; } = 0;
        public double RawLevel { get; set; } = 0;
        public ulong Experience { get; set; } = 0;
        public DateTime Typing { get; set; } // Make a tick in EventDependency.cs
        public DateTime LastSent { get; set; }

        public void TryUpdateLevel() {}
        public double GetLevel () {double tmp = 0; return tmp;}
        public double GetRawLevel() {double tmp = 0; return tmp;}
        public double LevelPercentile() {double tmp = 0; return tmp;}
        public double ExperienceOnLevel(double l) {double tmp = 0; return tmp;}
        public double ExperienceBetweenLevels(double a, double b) {double tmp = 0; return tmp;}
        public double RemainderToLevel(double l) {double tmp = 0; return tmp;}
        public ulong DetermineTimeSpent()
        {
            ulong tmp = 0;
            // make a tick system, checking when a user started typing.
            // upon it starting, update typestart value
            // check the length spent to a message being sent, and transfer it into its multiplier.

            return tmp;
        }
        public ulong DetermineCharAmount()
        {
            ulong tmp = 0;
            // Check the length of the text
            // if there's a repeat of multiple characters, deduct a heavy multiplier.
            // check if there's breaks, punctuations, etc.
            // determine base amount based on length AND if not spamming.
            return tmp;
        }
        public ulong DetermineSpam()
        {
            ulong tmp = 0;
            // Check the length of the text
            // if there's a repeat of multiple characters, deduct a heavy multiplier.
            return tmp;
        }

        public void Reward(ulong amt)
        {
            Experience += amt;
            TryUpdateLevel();
        }

        public void Calculate(SocketMessage m)
        {
            ulong isSpamming = DetermineSpam();// Check when last sent was, check if spamming. if spamming, constantly lower booster.
            ulong textWritten = DetermineCharAmount();// Check the amount of text written, and check if they're spam letters.
            ulong timeWritten = DetermineTimeSpent();// check the amount of time they spent writing it.
            ulong based = 2;// base exp from a message sent is 2 exp.
            ulong exp = based * timeWritten * textWritten * isSpamming;// set experience amount.
            Reward(exp); // add to current experience// try update level.
        } // Determine the exp earned from a sent message.
    }

    public class LocalData : IData
    {
        public LocalData() {}

        public int Level { get; set; } = 0;
        public double RawLevel { get; set; } = 0;
        public ulong Experience { get; set; } = 0;
        public ulong Prestige { get; set; } = 0;
        public ActiveData Active { get; set; } = new ActiveData();

        public void Reward(ulong amt)
        {
            Experience += amt;
            TryUpdateLevel();
        }

        public void TryUpdateLevel() {}
        public double GetLevel () {double tmp = 0; return tmp;}
        public double GetRawLevel() {double tmp = 0; return tmp;}
        public double LevelPercentile() {double tmp = 0; return tmp;}
        public double ExperienceOnLevel(double l) {double tmp = 0; return tmp;}
        public double ExperienceBetweenLevels(double a, double b) {double tmp = 0; return tmp;}
        public double RemainderToLevel(double l) {double tmp = 0; return tmp;}
    }

    public class GlobalData : IData
    {
        public GlobalData() {}

        public GlobalData(SocketGuild g)
        {
            Local.Add(g.Id, new LocalData());
        }

        public int Level { get; set; } = 0;
        public double RawLevel { get; set; } = 0;
        public ulong Prestige { get; set; } = 0;
        public ulong Experience { get; set; } = 0;
        public Dictionary<ulong, LocalData> Local {get; set;} = new Dictionary<ulong, LocalData>();

        public double PiecewisePrestige()
        {
            if (Prestige > 25) return 2.5;
            else if (Prestige >= 1 && Prestige <= 25) return Prestige/10;
            else return 0.1;
        }

        public void TryUpdateLevel()
        {
            RawLevel = GetRawLevel();
            Level = (int)GetLevel();
        }

        public double GetLevel() =>
            Math.Floor(RawLevel);

        // Returns the raw level of an account.
        public double GetRawLevel() =>
            Math.Sqrt(Experience / (10 * PiecewisePrestige()));

        // Returns the completion percent of a user's current level.
        public double LevelPercentile() =>
            (RawLevel - Level) * 100;

        // Returns the experience needed to be at a specified level.
        public double ExperienceOnLevel(double a) =>
            10 * PiecewisePrestige() * Math.Pow(a, 2);

        // Returns the amount of experience required between two levels.
        public double ExperienceBetweenLevels(double a, double b) =>
            ExperienceOnLevel(b) - ExperienceOnLevel(a);

        public double RemainderToLevel(double a) =>
                ExperienceOnLevel(a) - Experience;


        // Custom methods based on core methods.

        // Experience needed from your level to the next.
        public double ExperienceToNextLevel() =>
            ExperienceBetweenLevels(Level, Level + 1);

        public double TotalExperienceToNextLevel()
            => ExperienceBetweenLevels(0, Level + 1);

        // Experience needed to reach level 100 from your current level.
        public double RemainderExperience() =>
            RemainderToLevel(100);

        /// <summary>
        /// Returns the amount of experience needed to reach the next level.
        /// </summary>
        public double RemainderToNextLevel() =>
            RemainderToLevel(Level + 1);

        /// <summary>
        /// Grants the account and the server that it's in a specified amount of experience.
        /// </summary>
        public void Reward(Server s, ulong amt)
        {
            Experience += amt;
            TryUpdateLevel();
            if (!s.Config.TrackLocal) return;
            Reward(s.Id, amt);
        }

        private void Reward(ulong id, ulong amt)
        {
            TryAddLocal(id);
            Local[id].Reward(amt);
        }

        public void Calculate(SocketGuild g, SocketMessage m) =>
            GetLocal(g).Active.Calculate(m);

        public void Calculate(Server s, SocketMessage m) =>
            GetLocal(s).Active.Calculate(m);

        public LocalData GetLocal(SocketGuild g) =>
            GetLocal(g.Id);

        public LocalData GetLocal(Server s) =>
            GetLocal(s.Id);

        public LocalData GetLocal(ulong id)
        {
            TryAddLocal(id);
            Local.TryGetValue(id, out LocalData local);
            return local;
        }

        public void TryAddLocal(SocketGuild g) =>
            TryAddLocal(g.Id);

        public void TryAddLocal(Server s) =>
            TryAddLocal(s.Id);

        public void TryAddLocal(ulong id)
        {
            if (!Local.Keys.Contains(id))
                Local.Add(id, new LocalData());
        }
    }
}