﻿using Orikivo.Desync;
using System;
using System.Text;

namespace Orikivo
{
    internal static class HuskHandler
    {
        internal static string ViewStatus(User user, Husk husk)
        {
            bool showTooltips = user.Config?.Tooltips ?? false;
            var status = new StringBuilder();

            status.AppendLine($"> **HP**: **{husk.Status.Health}**/{husk.Attributes.MaxHealth}");
            
            if (showTooltips)
                status.AppendLine("> Represents your current health pool. This determines how many hits you can take before being desynchronized.\n");

            status.AppendLine($"> **Speed**: **{husk.Status.Speed}**m/s");

            if (showTooltips)
                status.AppendLine("> Represents your current travel speed. This determines how much time it takes for you to travel in open areas, such as a **Sector** or **Field**.\n");

            status.AppendLine($"> **Sight**: **{husk.Status.Sight}**mpp");

            if (showTooltips)
                status.AppendLine("> Represents your current view distance. This determines how many locations are visible from your current position (in meters per pixel).\n");

            status.AppendLine($"> **Reach**: **{husk.Status.Reach}**mpp");

            if (showTooltips)
                status.AppendLine("> Represents the maximum distance at which you can interact with objects or locations.");


            status.AppendLine($"> **Exposure Resistance**: {Format.Counter(TimeSpan.FromMinutes(husk.Status.Exposure).TotalSeconds)}");

            if (showTooltips)
                status.AppendLine("> Represents a your current resistance to wild exposure. This determines how long you can remain outside of a **Sector** before you start taking damage.");

            return status.ToString();
        }
    }
}