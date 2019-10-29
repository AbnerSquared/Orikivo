namespace Orikivo
{
    /*
        public void ClearSlots()
        {
            GoldSlot = null;
            CurseSlot = null;
        }

        public void SetRisk(int risk)
        {
            risk = risk > 100 ? 100 : risk;
            risk = risk < 0 ? 0 : risk;
            Risk = risk;
        }

        public void SetEarn(int earn)
        {
            earn = earn > 20 ? 20 : earn;
            earn = earn < 2 ? 2 : earn;
            Earn = earn;
        }
    */
    public static class GimiAttribute
    {
        public static string GoldSlot = "attribute:gimi:gold_slot";
        public static string CurseSlot = "attribute:gimi:curse_slot";
        public static string WinDirection = "attribute:gimi:win_direction";
        public static string Risk = "attribute:gimi:risk";
        public static string Earn = "attribute:gimi:earn";
    }
}
