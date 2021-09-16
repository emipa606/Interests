using RimWorld;
using Verse;

namespace DInterestsCallings
{
    internal class ThoughtWorker_CompulsionUnmet : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            var compNeedDef = DefDatabase<NeedDef>.GetNamed("DCompulsionNeed");
            if (!(p.needs.TryGetNeed(compNeedDef) is Need_Compulsion compNeed))
            {
                return ThoughtState.Inactive;
            }

            var moodBuffForCurrentLevel = (int) compNeed.MoodBuffForCurrentLevel;
            if (moodBuffForCurrentLevel == (int) Need_Compulsion.MoodBuff.Neutral)
            {
                return ThoughtState.Inactive;
            }

            return ThoughtState.ActiveAtStage(2 - moodBuffForCurrentLevel);
        }
    }
}