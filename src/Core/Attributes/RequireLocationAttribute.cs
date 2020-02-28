using Orikivo.Desync;
using System;
using System.Linq;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireLocationAttribute : Attribute
    {
        public RequireLocationAttribute(LocationType type)
        {
            Type = type;
        }

        public RequireLocationAttribute(string id, params string[] rest)
        {
            Id = id;
            Ids = rest;
        }

        public RequireLocationAttribute(ConstructTag tag)
        {
            Type = LocationType.Construct;
            Tag = tag;
        }

        // if you want the action to be bound a grouped type.
        public LocationType? Type { get; set; }

        public ConstructTag? Tag { get; set; }

        // if you want an action to be bound to an id
        public string Id { get; set; }
        public string[] Ids { get; set; }

        public bool Contains(string id)
            => Id == id || Ids.Contains(id);

        public bool Check(Locator location)
        {
            if (Tag.HasValue)
            {
                if (location.GetInnerType() == LocationType.Construct)
                    return location.GetConstruct().Tag.HasFlag(Tag.Value);
                else
                    return false;
            }
            if (Type.HasValue)
            {
                return Type.Value.HasFlag(location.GetInnerType());
            }
            else
            {
                return Contains(location.GetInnerId());
            }
        }
    }
}
