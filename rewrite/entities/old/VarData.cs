using Newtonsoft.Json;
using System;

namespace Orikivo
{
    // Create conversion variabilities for varying types.
    // >> MeritData, StatData, UpgradeData, CooldownData, AttributeData, BoosterData,
    // BalanceData, DebtData, TokenData, ExpData, LevelData, AscentData 
    public struct VarData
    {
        public static VarData FromCurrency(CurrencyType type, ulong value)
        {
            string currencyType = type switch
            {
                CurrencyType.Generic => "balance",
                CurrencyType.Vote => "token",
                CurrencyType.Debt => "debt"
            };

            return new VarData(currencyType, value);
        }
        public static VarData FromExp(ulong exp)
        {
            return new VarData(VarBase.VAR_EXP, exp);
        }

        //public static VarData FromLevel(ulong exp)
        //{
        //    return new VarData(VarBase.VAR_LEVEL, OriSource.ConvertExpToLevel(exp));
        //}

        //public static VarData FromCooldown(CooldownData cooldown)
        //{
        //    return new VarData(cooldown.Id, cooldown);
        //}

        [JsonConstructor]
        internal VarData(string id, object value)
        {
            Type = VarBase.GetType(id);
            Group = VarBase.GetGroup(id);
            PropertyName = VarBase.GetPropertyName(id);
            ValueType = VarBase.GetCheckType(Type);

            if (!VarBase.IsValueSafe(Type, value))
                throw new ArgumentException("The value specified for this Var isn't safe.", nameof(value));

            Value = value;
        }

        [JsonProperty("id")]
        public string Id
        {
            get
            {
                string id = Type;

                if (!string.IsNullOrWhiteSpace(Group))
                {
                    id += '/';
                    id += Group;
                }

                if (!string.IsNullOrWhiteSpace(PropertyName))
                {
                    id += '/';

                    if (!string.IsNullOrWhiteSpace(Group))
                    {
                        id += Group;
                        id += ':';
                    }

                    id += PropertyName;
                }

                return id;
            }
        }

        [JsonIgnore]
        public string Type { get; }

        [JsonIgnore]
        public string Group { get; }

        [JsonIgnore]
        public string PropertyName { get; }

        [JsonIgnore]
        public Type ValueType { get; }

        [JsonProperty("value")]
        public object Value { get; internal set; }
    }
}
