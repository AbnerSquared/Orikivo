using Newtonsoft.Json;

namespace Orikivo
{
    // used to set how the gimi command works for you
    public class GimiInfo
    {
        [JsonConstructor]
        public GimiInfo(int risk, int earn, int? goldSlot = null, int? curseSlot = null)
        {
            Risk = risk;
            Earn = earn;
            GoldSlot = goldSlot;
            CurseSlot = curseSlot;
        }

        public GimiInfo()
        {
            Risk = 50;
            Earn = 10;
        }

        [JsonProperty("risk")]
        public int Risk { get; private set; }

        [JsonProperty("earn")]
        public int Earn { get; private set; }

        [JsonProperty("gold_slot")]
        public int? GoldSlot { get; private set; }
        
        [JsonProperty("curse_slot")]
        public int? CurseSlot { get; private set; }

        [JsonProperty("win_dir")]
        public bool? WinDir { get; private set; }

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

        internal void SetGoldSlot(int slot)
            => GoldSlot = slot;

        internal void SetCurseSlot(int slot)
            => CurseSlot = slot;

        internal void SetWinDir(bool dir)
            => WinDir = dir;
    }
}
