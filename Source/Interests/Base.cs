using Verse;

namespace DInterestsCallings
{
    [StaticConstructorOnStartup]
    public static class Base
    {
        static Base()
        {
            InterestsSettings.WriteAll();
        }
    }
}