using System;

namespace Orikivo
{
    /* Stores all merit, item, cooldown, booster, etc. info */
    public class OriSource // GetOrAddUser()... GetOrLoadJson();
    {
        public TextInfo[] Texts { get; }

        public MeritInfo[] Merits { get; }

        public CooldownInfo[] Cooldowns { get; }

        public ItemInfo[] Items { get; }

        public UpgradeInfo[] Upgrades { get; }

        public BoosterInfo[] Boosters { get; }

        public static TextInfo GetInfoForText(string key)
        { throw new NotImplementedException(); }

        public static string GetText(string key)
        { throw new NotImplementedException(); }

        public static int ConvertExpToLevel(ulong exp)
        {
            // EXP_TO_LEVEL_EQ =>
            // LEVEL_TO_EXP_EQ =>
            throw new NotImplementedException();
        }

        public static CooldownInfo GetCooldownInfo(string cooldownId)
        {
            string varType = VarBase.GetType(cooldownId); // cooldown/
            if (varType != VarBase.VAR_COOLDOWN)
                throw new ArgumentException($"Var defining type is invalid. Expected '{VarBase.VAR_COOLDOWN}', got '{varType}.'", nameof(cooldownId));

            string cooldownType = VarBase.GetGroup(cooldownId);

            return GetCooldownInfoFor(cooldownType);
        }

        public static CooldownInfo GetCooldownInfoFor(string cooldownType)
        {
            throw new NotImplementedException(); // TODO: figure out how to search for the matching cooldown...
        }

        public static MeritInfo GetMeritInfo(string meritId)
        {
            throw new NotImplementedException();
        }

        public static bool MeetsMeritCriteria(MeritInfo merit, VarData[] datas)
        {
            int matches = 0;
            foreach(VarCriterion criterion in merit.Criteria)
            {
                foreach (VarData data in datas)
                {
                    if (data.Id == criterion.Id)
                    {

                        if (VarBase.Match(data.Type, data.Value, criterion.CheckValue, criterion.Match))
                        {
                            matches++;
                            continue;
                            // A var successfully matches a single merit criterion.
                            // return false the moment a matching var fails to match.
                        }
                        else
                            return false;
                    }
                }
            }

            return matches == merit.Criteria.Length;
        }
    }
}
