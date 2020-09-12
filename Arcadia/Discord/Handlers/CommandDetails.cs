using System.Text;

namespace Arcadia
{
    public static class CommandDetails
    {
        public static string WriteTick()
        {
            var info = new StringBuilder();
            info.AppendLine($"> 🎰 **Casino**");
            info.AppendLine("> Double Machine\n");

            info.AppendLine($"This machine offers high risk with high reward, allowing you to gain a massive amount of {Icons.Chips} **Chips** in only a single run.\n");
            info.AppendLine($"> **Step 1: Generation**");
            info.AppendLine("In this phase, the machine rolls a tick chance. As long as the generated tick is below the current **Chance**, the machine reduces the **Chance** by **1**%, and continues generating ticks. Once the generated tick fails, the machine is stopped, returning the **Actual**.");
            info.AppendLine("\n> **Step 2: Calculation**");
            info.AppendLine("Once the **Actual** is found, it compares it with your **Method** to see if your **Guess** was successful.");
            info.AppendLine("\nIf your **Method** is exact:\n- If the **Actual** is equal to the **Guess**, your result is marked as **Exact**, otherwise it's marked as a **Loss**.");
            info.AppendLine("\nOtherwise, if your **Method** is below:\n- If the **Actual** is greater than or equal to the **Guess**, your result is marked as a **Win** (**Exact** if the **Actual** is equal to the **Guess**).");
            info.AppendLine("\n> **Step 3: Reward**");
            info.AppendLine($"If you were given a **Loss**, this returns a reward of {Icons.Chips} **0**.");
            info.AppendLine($"\nOtherwise, if you were given a **Win** or **Exact**, you are given a reward of:\n- {Icons.Chips} `floor(Bet * (2 ^ Actual) * (Method == Exact ? 1.5 : 1))`");

            return info.ToString();
        }

        public static string WriteGetChips()
        {
            var info = new StringBuilder();
            info.AppendLine("> **Chip Conversion**");
            info.AppendLine($"> {Icons.Balance} **1** ≈ {Icons.Chips} **{MoneyConvert.ToChips(1)}**");
            info.AppendLine($"> {Icons.Balance} **10** ≈ {Icons.Chips} **{MoneyConvert.ToChips(10)}**");
            info.AppendLine($"> {Icons.Balance} **100** ≈ {Icons.Chips} **{MoneyConvert.ToChips(100)}**");
            info.AppendLine("\nSpecify an amount to convert your **Orite** into **Chips**, for use at the **Casino**.");
            return info.ToString();
        }

        public static string WriteGimi()
        {
            var info = new StringBuilder();
            info.AppendLine($"> 🎰 **Casino**");
            info.AppendLine("> Gimi\n");
            info.AppendLine($"This is a built-in machine that provides {Icons.Balance} **Orite** or {Icons.Debt} **Debt** at random.\n");

            info.AppendLine($"> **Step 1: Generation**");
            info.AppendLine("In this phase, a **Seed** is rolled based on your **Risk** (default is **50**%) to determine your win state. If the **Seed** that was rolled landed within the range specified by your **Win Direction** (default is **Above**), you are given a **Win**. Otherwise, your result is a **Loss**.");
            info.AppendLine($"\nThe **Seed** is then checked for a match in the collection of specified golden slots. If a match was successful, your result is set to **Gold**. If a match failed, your result is left alone unless your **Seed** is 1 off of the **Gold**, which sets your result to **Curse**.");
            info.AppendLine($"\n> **Step 2: Reward**");
            info.AppendLine($"If your result was **Gold**, you are given {Icons.Balance} **50** and a **Pocket Lawyer**.\nIf your result was **Curse**, you are given {Icons.Debt} **200**.\nOtherwise, you are given a random amount of {Icons.Balance} **Orite** ({Icons.Debt} **Debt** if **Loss**) based on your **Risk**.");
            return info.ToString();
        }

    }
}