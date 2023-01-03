using Arcadia.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Autocompletion
{
    public static class CollectionHelper
    {
        public static int GetScore(string input, IModel<string> model)
        {
            int idScore = 1;
            int nameScore = 0;
            for (int i = 0; i < input.Length; i++)
            {
                char a = input[i];

                if (i < model.Id.Length)
                {
                    char b = model.Id[i];
                    if (a.Equals(b))
                    {
                        if (char.IsLetter(a) && char.IsLetter(b)) idScore += 2;
                        else idScore++;
                    }
                    else if (string.Equals(a.ToString(), b.ToString(), StringComparison.OrdinalIgnoreCase)) idScore++;

                }
                else idScore--;

                if (i < model.Name.Length)
                {
                    char b = model.Name[i];
                    if (a.Equals(b))
                    {
                        if (char.IsLetter(a) && char.IsLetter(b)) idScore += 2;
                        else nameScore++;
                    }
                    else if (string.Equals(a.ToString(), b.ToString(), StringComparison.OrdinalIgnoreCase)) nameScore++;
                }
                else nameScore--;
            }

            return Math.Max(idScore, nameScore);
        }

        public static IEnumerable<IModel<string>> OrderByScore(string input, IEnumerable<IModel<string>> models)
        {
            return models.OrderByDescending(x => GetScore(input, x));
        }
    }
}
