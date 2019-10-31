using Newtonsoft.Json;
using System;

namespace Orikivo
{
    public struct VarCriterion
    {
        [JsonConstructor]
        internal VarCriterion(string id, object value, VarMatch? match)
        {
            Type = VarBase.GetType(id);
            Group = VarBase.GetGroup(id);
            PropertyName = VarBase.GetPropertyName(id);
            CheckType = VarBase.GetCheckType(Type);

            if (!VarBase.MakeCheckSafe(Type, value))
                throw new ArgumentException("The value specified for this Var isn't safe.", nameof(value));

            CheckValue = value;
            Match = match ?? VarMatch.Equals;
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

        [JsonProperty("check_value")]
        public object CheckValue { get; }
        
        [JsonIgnore]
        public Type CheckType { get; }

        [JsonProperty("match")]
        public VarMatch Match { get; }
    }
}
