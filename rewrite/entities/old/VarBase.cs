using Orikivo.Unstable;
using System;

namespace Orikivo
{
    /* Helps with constructing parsing Vars. */
    public static class VarBase
    {
        // if two object dataValues match.
        public static bool Match(string varType, object value, object checkValue, VarMatch match = VarMatch.Equals)
        {
            Type valueType = GetValueType(varType);
            Type checkType = GetCheckType(varType);

            bool valueMatchesType = value.GetType() == valueType;
            bool checkMatchesType = checkValue.GetType() == checkType;
            if (!valueMatchesType && !varType.EqualsAny(VAR_MERIT, VAR_COOLDOWN, VAR_BOOSTER) || !checkMatchesType)
                throw new ArgumentException("Not all of the inbound values match their specified Var type.");

            if (varType.EqualsAny(VAR_STAT, VAR_UPGRADE, VAR_COOLDOWN, VAR_ATTRIBUTE,
                VAR_BALANCE, VAR_DEBT, VAR_TOKEN, VAR_EXP, VAR_LEVEL, VAR_ASCENT))
                return MatchObjects(valueType, value, checkValue, match);

            if (varType.EqualsAny(VAR_MERIT, VAR_COOLDOWN, VAR_BOOSTER))
                return (bool)checkValue ? valueMatchesType : value.GetType() == null;

            throw new ArgumentException("Unknown Var defining type.", nameof(varType));
        }

        public static bool MatchObjects(Type valueType, object value, object matchValue, VarMatch match)
        {
            bool typesMatch = value.GetType() == valueType && matchValue.GetType() == valueType;

            if (!typesMatch)
                throw new Exception("The specified values don't match the given value type.");

            if (!valueType.EqualsAny(typeof(ulong), typeof(int)))
                return value == matchValue;

            ulong uValue = Convert.ToUInt64(value);
            ulong uMatchValue = Convert.ToUInt64(matchValue);

            return match switch
            {
                VarMatch.Equals => uValue == uMatchValue,
                VarMatch.NotEquals => uValue != uMatchValue,
                VarMatch.Greater => uValue > uMatchValue,
                VarMatch.Lesser => uValue < uMatchValue,
                VarMatch.GreatOrEquals => uValue >= uMatchValue,
                VarMatch.LessOrEquals => uValue <= uMatchValue,
                _ => throw new ArgumentException("Unknown VarMatch type specified.", nameof(match))
            };
        }

        // command:help+0
        // command:main_alias+priority
        public const string COMMAND_SUBST = "command:";
        public const string VAR_MERIT = "merit";
        public const string VAR_STAT = "stat";
        public const string VAR_UPGRADE = "upgrade";
        public const string VAR_ATTRIBUTE = "attribute";
        public const string VAR_BOOSTER = "booster";
        public const string VAR_COOLDOWN = "cooldown";
        public const string VAR_BALANCE = "balance";
        public const string VAR_DEBT = "debt";
        public const string VAR_TOKEN = "token";
        public const string VAR_EXP = "exp";
        public const string VAR_LEVEL = "level";
        public const string VAR_ASCENT = "ascent";

        public const string GROUP_GLOBAL = "global";
        
        public static int BoolToInt(bool value)
            => value ? 1 : 0;

        /* Checking types when using criteria */
        public static readonly Type MeritCheckType = typeof(int); // BOOLEAN
        public static readonly Type StatCheckType = typeof(ulong);
        public static readonly Type UpgradeCheckType = typeof(int);
        public static readonly Type CooldownCheckType = typeof(int); // BOOLEAN
        public static readonly Type AttributeCheckType = typeof(ulong);
        public static readonly Type BoosterCheckType = typeof(int); // BOOLEAN
        public static readonly Type BalanceCheckType = typeof(ulong);
        public static readonly Type DebtCheckType = typeof(ulong);
        public static readonly Type TokenCheckType = typeof(ulong);
        public static readonly Type ExpCheckType = typeof(ulong);
        public static readonly Type LevelCheckType = typeof(int);
        public static readonly Type AscentCheckType = typeof(int);

        /* Literal value types that are to be expected */
        public static readonly Type MeritValueType = typeof(MeritData); // Non-Matchable, compare at Match
        public static readonly Type StatValueType = typeof(ulong);
        public static readonly Type UpgradeValueType = typeof(int);
        public static readonly Type CooldownValueType = typeof(CooldownData); // Non-Matchable compare at Match
        public static readonly Type AttributeValueType = typeof(ulong);
        public static readonly Type BoosterValueType = typeof(BoostData); // Non-Matchable compare at Match
        public static readonly Type BalanceValueType = typeof(ulong);
        public static readonly Type DebtValueType = typeof(ulong);
        public static readonly Type TokenValueType = typeof(ulong);
        public static readonly Type ExpValueType = typeof(ulong);
        public static readonly Type LevelValueType = typeof(int);
        public static readonly Type AscentValueType = typeof(int);

        public static readonly string[] VarTypes = new string[]
        {
            VAR_MERIT, VAR_STAT, VAR_UPGRADE, VAR_ATTRIBUTE, VAR_BOOSTER, VAR_COOLDOWN,
            VAR_BALANCE, VAR_DEBT, VAR_TOKEN, VAR_EXP, VAR_LEVEL, VAR_ASCENT
        };

        public static string GetType(string id)
        {
            string possibleType = id.Split('/')[0];
            if (possibleType.EqualsAny(VarTypes))
                return possibleType;
            throw new ArgumentException("type isn't any of the specified types.", nameof(id));
        }

        public static string GetGroup(string id)
        {
            string[] parsable = id.Split('/')[1].Split(':');
            return parsable.Length > 1 ? parsable[0] : GROUP_GLOBAL;
        }

        public static string GetPropertyName(string id)
        {
            string[] parsable = id.Split('/')[1].Split(':');
            return parsable.Length > 1 ?
                parsable[1] : parsable.Length == 1 ?
                parsable[0] : string.Empty;
        }

        public static Type GetCheckType(string varType)
        {
            return varType switch
            {
                VAR_MERIT => MeritCheckType,
                VAR_STAT => StatCheckType,
                VAR_UPGRADE => UpgradeCheckType,
                VAR_ATTRIBUTE => AttributeCheckType,
                VAR_BOOSTER => BoosterCheckType,
                VAR_COOLDOWN => CooldownCheckType,
                VAR_BALANCE => BalanceCheckType,
                VAR_DEBT => DebtCheckType,
                VAR_TOKEN => TokenCheckType,
                VAR_EXP => ExpCheckType,
                VAR_LEVEL => LevelCheckType,
                VAR_ASCENT => AscentCheckType,
                _ => throw new ArgumentException("An unknown VarType value was passed.", nameof(varType))
            };
        }

        public static Type GetValueType(string varType)
        {
            return varType switch
            {
                VAR_MERIT => MeritValueType,
                VAR_STAT => StatValueType,
                VAR_UPGRADE => UpgradeValueType,
                VAR_ATTRIBUTE => AttributeValueType,
                VAR_BOOSTER => BoosterValueType,
                VAR_COOLDOWN => CooldownValueType,
                VAR_BALANCE => BalanceValueType,
                VAR_DEBT => DebtValueType,
                VAR_TOKEN => TokenValueType,
                VAR_EXP => ExpValueType,
                VAR_LEVEL => LevelValueType,
                VAR_ASCENT => AscentValueType,
                _ => throw new ArgumentException("An unknown VarType value was passed.", nameof(varType))
            };
        }

        public static object GetDefaultCheck(string varType)
        {
            return varType switch
            {
                VAR_MERIT => 1,
                VAR_STAT => 0,
                VAR_UPGRADE => 0,
                VAR_ATTRIBUTE => 0,
                VAR_BOOSTER => 1,
                VAR_COOLDOWN => 1,
                VAR_BALANCE => 0,
                VAR_DEBT => 0,
                VAR_TOKEN => 0,
                VAR_EXP => 0,
                VAR_LEVEL => 0,
                VAR_ASCENT => 0,
                _ => throw new ArgumentException("An unknown VarType value was passed.", nameof(varType))
            };
        }

        public static bool MakeCheckSafe(string varType, object value)
        {
            Type valueType = GetCheckType(varType);
            if (value.GetType() == valueType)
            {
                if (varType.EqualsAny(VAR_MERIT, VAR_BOOSTER, VAR_COOLDOWN))
                {
                    if ((int)value > 1)
                        value = 1;
                }

                return true;
            }
            if (value.GetType() == null)
            {
                value = GetDefaultCheck(varType);
                return true;
            }

            return false;
        }

        public static bool IsValueSafe(string varType, object value)
        {
            Type valueType = GetValueType(varType);
            return value.GetType() == valueType;
        }
    }
}
