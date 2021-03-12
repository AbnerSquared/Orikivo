using Arcadia.Modules;
using Orikivo;
using System.Text;

namespace Arcadia
{
    public static class EquipHelper
    {
        public static string ViewBoosters(ArcadeUser user)
        {
            var result = new StringBuilder();

            result.AppendLine($"> {Icons.Boosters} **Boosters**");

            foreach(BoostData data in user.Boosters)
            {
                string row = $" • **{BoostViewer.WriteName(data)}**: {Format.Percent(data.Rate)} **{data.Type.ToString()}** income";

                result.AppendLine(row);
            }

            return result.ToString();
        }

        public static string ViewCardEquipment(ArcadeUser user)
        {
            var result = new StringBuilder();

            result.AppendLine($"> {Icons.Card} **Card**");
            result.AppendLine($" • **Layout**: {user.Card.Layout.ToString()}");
            result.AppendLine($" • **Font**: {user.Card.Font.ToString()}");
            result.Append($" • **Color Palette: {user.Card.Palette.ToString()}**");

            return result.ToString();
        }

        public static string View(ArcadeUser user)
        {
            var result = new StringBuilder();

            if (user.Boosters.Count > 0)
                result.AppendLine(ViewBoosters(user));

            result.AppendLine(ViewCardEquipment(user));

            return result.ToString();
        }
    }
}