using System.Collections.Generic;

namespace Orikivo
{
    // This the base class of a game for orikivo.
    // This calculates exp gain amount, if the game allows wagering // ExperienceMechanics //
    // the achievements // IAchievement
    // if the game has item drops // ItemDrop
    // if the game can use inventory items // IUsable
    public class InstantGame
    {
        public string Name { get; set; }
    }

    public enum RpsType
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3
    }

    public class RpsResult
    {
        public ulong HostId { get; set; }
        public ulong ChallengerId { get; set; }
        public RpsType HostType { get; set; }
        public RpsType ChallengerType { get; set; }
        public ulong WinnerId { get; set; }

    }

    // rockpaperscissors
    // paper beats rock     | paper dies from scissors
    // rock beats scissors  | rock dies from paper
    // scissors beats paper | scissors dies from rock

    // a class for rps

    // rps <type>
    // rpsc or rpschallenge <user> | if the other user has challenged you already, it starts.
      
    public class RockPaperScissorsGame
    {
        //public RpsResult PlayBot(RpsType type)
        //{

        //}

        //public RpsResult PlayUser(RpsType type)
        //{

        //}
    }
}