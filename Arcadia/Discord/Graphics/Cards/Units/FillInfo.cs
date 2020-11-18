using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class FillInfo : BaseFillInfo
    {
        public FillInfo() { }

        public FillInfo(BaseFillInfo info)
        {
            if (info == null)
                return;

            Primary = info.Primary;
            Secondary = info.Secondary;
            Mode = info.Mode;
            FillPercent = info.FillPercent;
            Direction = info.Direction;
        }

        public GammaPalette Palette { get; set; } = GammaPalette.Default;

        public void SetBaseInfo(BaseFillInfo baseInfo)
        {
            if (baseInfo == null)
                return;

            if (baseInfo.Primary != 0)
                Primary = baseInfo.Primary;

            if (baseInfo.Secondary.HasValue)
                Secondary = baseInfo.Secondary;

            if (baseInfo.Mode != FillMode.None)
                Mode = baseInfo.Mode;

            if (baseInfo.FillPercent.HasValue)
               FillPercent = baseInfo.FillPercent;

            if (baseInfo.Direction != 0)
                Direction = baseInfo.Direction;
        }
    }
}
