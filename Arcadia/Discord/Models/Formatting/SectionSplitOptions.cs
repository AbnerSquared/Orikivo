﻿namespace Arcadia
{
    public enum SectionSplitOptions
    {
        Reset = 1, // If a section is cropped, the next page will rewrite the section as new
        ForceHeader = 2, // If a section is cropped, the next page will show the header for the section and finish its content
        Ignore = 3, // If a section is cropped, The remaining text is written out as normal.
    }
}