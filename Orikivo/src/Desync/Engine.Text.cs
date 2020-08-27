using System;
using System.Linq;
using System.Text;

namespace Orikivo.Desync
{
    // This handles writing everything that the engine shows.
    public static partial class Engine
    {
        public static string WriteVisibleCharacters(Husk husk)
        {
            var npcs = GetVisibleCharacters(husk);

            if (npcs?.Count() > 0)
            {
                var result = new StringBuilder();
                var summaries = npcs.Select(x => $"> `{x.Id}` • {x.Name}");

                result.AppendLine($"**Available NPCs** ({husk.Location.GetInnerName()}):");
                result.AppendJoin("\n", summaries);

                return result.ToString();
            }

            return $"There isn't anyone to talk to in **{husk.Location.GetInnerName()}**.";
        }

        public static string WriteVisibleLocations(Husk husk, HuskBrain brain)
        {
            Location location = husk.Location.GetLocation();
            var locations = new StringBuilder();

            switch (location.Type)
            {
                case LocationType.Area:
                    var area = location as Area;
                    locations.AppendLine($"**Available Locations** ({area.Name}):");

                    if (area.Constructs?.Count > 0)
                    {
                        locations.AppendLine($"**Available Locations** ({area.Name}):");
                        locations.AppendJoin("\n", area.Constructs.Select(x => $"> `{x.Id}` • {x.Name}"));
                        return locations.ToString();
                    }

                    return "There isn't anything close by. Maybe try going to a different area?";

                case LocationType.Sector:
                    var sector = location as Sector;
                    var structures = GetVisibleStructures(husk, sector);

                    if (sector.Areas?.Count == 0 && structures?.Count() == 0)
                        return "There isn't anything close by. Try looking around!";

                    if (sector.Areas?.Count > 0)
                    {
                        locations.AppendLine($"**Available Areas** ({sector.Name}):");
                        locations.AppendJoin("\n", sector.Areas.Select(x => $"> `{x.Id} • {x.Name}`"));
                    }

                    if (structures?.Count() > 0)
                    {
                        locations.AppendLine();
                        locations.AppendLine($"**Points of Interest**:");

                        var summaries = structures.Select(x =>
                        brain.HasDiscoveredRegion(x.Id)
                        ? $"> `{x.Id}` • {x.Name}"
                        : $"> `({x.Shape.Position.X}, {x.Shape.Position.Y})` • Unknown Structure");

                        locations.AppendJoin("\n", summaries);
                    }

                    if (brain.Memorials?.Where(x => x.Location.Id == sector.Id).Count() > 0)
                    {
                        locations.Append("\n");
                        var summaries = brain.Memorials.Select(x =>
                        $">` ({x.Location.X}, {x.Location.Y})` • Memorial");

                        locations.AppendJoin("\n", summaries);
                    }

                    return locations.ToString();

                default:
                    throw new Exception("The specified Husk is currently at an invalid location.");
            }
        }
        /* You are currently travelling to **(32, 32)** [55%]
         
            [55%] => the % of completion that you have on your travels
            **{0}** => the location you are currently heading towards.
             
             */

        public static string WriteLocationInfo(string id, bool isDestination = false)
        {
            Location location = World.Find(id);

            if (location == null)
                return "You are currently in the **void**.";

            var summary = new StringBuilder();

            summary.Append(isDestination ? "You are currently travelling to " : "You are currently in ");

            if (location.Type == LocationType.Construct)
            {
                if ((location as Construct).Tag == ConstructType.Floor)
                {
                    summary.Append($"**{location.Name}** at **{location.GetParent()?.Name} - {(location as Floor).Index}F**");
                }
                else
                {
                    summary.Append($"**{location.Name}**");
                }
            }
            else
            {
                summary.Append($"**{location.Name}**");
            }

            Location parent = location.GetParent();
            int index = 0;

            if (parent != null)
            {
                summary.Append(" (");
                int insertPoint = summary.Length;

                while (parent != null)
                {
                    if (index > 0)
                    {
                        summary.Insert(insertPoint, $"{parent.Name}, ");
                    }
                    else
                    {
                        summary.Append(parent.Name);
                    }

                    parent = parent.GetParent();
                    index++;
                }

                summary.Append(")");
            }

            /*
            if (isDestination)
            {
                // This is where incorporating the % of completion should take place.
            }
            */

            summary.Append(".");

            return summary.ToString();
        }
    }
}
