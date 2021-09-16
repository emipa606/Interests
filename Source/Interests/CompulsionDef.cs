using DInterests;
using RimWorld;
using Verse;

namespace DInterestsCallings
{
    internal class CompulsionDef : InterestDef
    {
        public override void HandleLearn(ref float xp, SkillRecord sk, Pawn pawn, ref bool direct)
        {
            if (xp < 0 || direct)
            {
                return;
            }

            var compNeedDef = DefDatabase<NeedDef>.GetNamed("DCompulsionNeed");

            var compNeed = pawn.needs.TryGetNeed(compNeedDef) as Need_Compulsion;
            if (compNeed == null)
            {
                Log.Error("Failed to find need associated with DCompulsionNeed");
            }

            compNeed?.Notify_Worked(xp);
        }
    }
}