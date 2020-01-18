using System;
using Orikivo.Drawing;

namespace Orikivo
{
    // REMOVAL FROM ORIKIVO.DRAWING; Too specific to be generalized.
    public class TimeCycle
    {
        // NOTE: These four color maps transition between each other to create a timely color system.
        public static GammaPalette SunrisePalette = new GammaPalette(0x15123D, 0x261948, 0x381B51, 0x4F285D, 0x653366, 0x894B78, 0xA96786, 0xC5748D);
        public static GammaPalette MeridianPalette = new GammaPalette(0x004F99, 0x0070B1, 0x008DC3, 0x2AAFDB, 0x5ED5ED, 0x7FE3EF, 0x99EDF5, 0xDBFCFC);
        public static GammaPalette DuskPalette = new GammaPalette(0x681467, 0x7F1A70, 0xA21E7B, 0xDB308E, 0xE95490, 0xEF6C94, 0xF87C90, 0xF59098);
        public static GammaPalette NightPalette = new GammaPalette(0x020A21, 0x091934, 0x0D213D, 0x00314F, 0x194A5A, 0x346061, 0x397773, 0x2E8982);

        public static GammaPalette FromUtcNow()
            => FromTime(DateTime.UtcNow);

        public static GammaPalette FromTime(DateTime time)
            => FromHour(GetHourFloatValue(time));

        public static GammaPalette FromHour(float hour) // TODO: Incorporate offsets, and incorporate Range.Markers (when ready).
        {
            TimeCycle cycle = Utc;

            if (hour <= cycle.NightEnd || hour > cycle.NightStart)
                return NightPalette;

            if (hour > cycle.NightEnd && hour <= cycle.Dawn)
                return GammaPalette.Merge(NightPalette, SunrisePalette, GetHourStrength(cycle.NightEnd, cycle.Dawn, hour));

            if (hour > cycle.Dawn && hour <= cycle.Sunrise)
                return GammaPalette.Merge(SunrisePalette, DuskPalette, GetHourStrength(cycle.Dawn, cycle.Sunrise, hour));

            if (hour > cycle.Sunrise && hour <= cycle.Meridian)
                return GammaPalette.Merge(DuskPalette, MeridianPalette, GetHourStrength(cycle.Sunrise, cycle.Meridian, hour));

            if (hour > cycle.Meridian && hour <= cycle.Sunset)
                return GammaPalette.Merge(MeridianPalette, DuskPalette, GetHourStrength(cycle.Meridian, cycle.Sunset, hour));

            if (hour > cycle.Sunset && hour <= cycle.Dusk)
                return GammaPalette.Merge(DuskPalette, SunrisePalette, GetHourStrength(cycle.Sunset, cycle.Dusk, hour));

            if (hour > cycle.Dusk && hour <= cycle.NightStart)
                return GammaPalette.Merge(SunrisePalette, NightPalette, GetHourStrength(cycle.Dusk, cycle.NightStart, hour));

            throw new Exception("The hour float value given is out of range.");
        }

        private static float GetHourStrength(float from, float to, float hour)
            => RangeF.Percent.Convert(0.00f, to - from, hour - from);

        public static float GetHourFloatValue(DateTime time)
        {
            float remHour = RangeF.Percent.Convert(0, 59, time.Minute);
            float hour = time.Hour + remHour;
            return hour;
        }

        private TimeCycle() { }

        // public static TimeCycle FromTimeZone(TimeZone zone) {}

        public static TimeCycle Utc = new TimeCycle { Dawn = 7.00f, Sunrise = 8.00f, Meridian = 12.00f, Sunset = 16.00f, Dusk = 17.00f, NightStart = 18.00f, NightEnd = 6.00f };

        public float Dawn { get; private set; }
        public float Sunrise { get; private set; }
        public float Meridian { get; private set; }
        public float Sunset { get; private set; }
        public float Dusk { get; private set; }
        public float NightStart { get; private set; }
        public float NightEnd { get; private set; }
    }
}
