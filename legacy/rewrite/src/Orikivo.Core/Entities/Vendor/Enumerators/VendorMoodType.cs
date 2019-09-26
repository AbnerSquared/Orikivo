namespace Orikivo
{
    public enum VendorMoodType
    {
        Neutral = 0, // has no effect on whatever questions/responses are placed.
        Happy = 1, // less likely to be offended/hurt from negative responses.
        Sad = 2, // more likely to be upset/offended from negative/offensive topics, and less likely to benefit from positive
        Angry = 3, // much more likely to be upset/offended, and less likely to benefit from positive responses.
    }
}