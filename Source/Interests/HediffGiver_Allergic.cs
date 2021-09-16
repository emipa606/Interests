using DInterests;
using Verse;

namespace DInterestsCallings
{
    public class HediffGiver_Allergic : HediffGiver
    {
        public float gainRate = 60.0f / 2500.0f / 10.0f; // 10 hours to 100%, 7 hours to anaphylaxis

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if (!pawn.IsColonist)
            {
                return;
            }

            var active = InterestBase.GetActiveSkill(pawn);
            if (active == null)
            {
                DecreaseAllergy(pawn);
                return;
            }

            var skill = pawn.skills.GetSkill(active);
            if (skill == null)
            {
                return;
            }

            var passion = (int) skill.passion;
            var allergic = InterestBase.interestList["DAllergic"];
            var hediffSet = pawn.health.hediffSet;
            var firstHediffOfDef = hediffSet.GetFirstHediffOfDef(hediff);
            if (passion != allergic &&
                firstHediffOfDef !=
                null) // pawn's active skill isn't causing an allergy, but they have the allergic hediff
            {
                DecreaseAllergy(pawn);
                //firstHediffOfDef.Severity -= gainRate;
            }
            else if (passion == allergic) // pawn's active skill is allergy causing
            {
                IncreaseAllergy(pawn);
            }

            base.OnIntervalPassed(pawn, cause);
        }

        public void IncreaseAllergy(Pawn pawn)
        {
            HealthUtility.AdjustSeverity(pawn, hediff, gainRate);
        }

        public void DecreaseAllergy(Pawn pawn)
        {
            HealthUtility.AdjustSeverity(pawn, hediff, -gainRate);
        }
    }
}