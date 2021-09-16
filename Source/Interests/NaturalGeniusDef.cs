using DInterests;
using RimWorld;
using Verse;

namespace DInterestsCallings
{
    internal class NaturalGeniusDef : InterestDef
    {
        public override void HandleTick(SkillRecord sr, Pawn pawn)
        {
            if (pawn.Awake())
            {
                sr.Learn(4.0f / learnFactor, true);
            }
        }
    }
}