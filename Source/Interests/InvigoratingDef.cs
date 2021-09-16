using DInterests;
using RimWorld;
using Verse;

namespace DInterestsCallings
{
    internal class InvigoratingDef : InterestDef
    {
        public override void HandleTick(SkillRecord sk, Pawn pawn)
        {
            var sd = InterestBase.GetActiveSkill(pawn);
            if (sd == null)
            {
                return;
            }

            var skill = pawn.skills.GetSkill(sd);
            if (skill != sk)
            {
                return;
            }

            if (!(pawn.needs.TryGetNeed(NeedDefOf.Rest) is Need_Rest restNeed))
            {
                Log.Error("Got null rest need, wat");
                return;
            }

            // Rest fall per 150 ticks is 150f*1.58333332E-05f * RestFallFactor * (modifier based on tiredness level);
            // Perfect equilibrium is 200f* 1.58333332E-05f
            var rc = restNeed.CurCategory;
            float factor;
            switch (rc)
            {
                case RestCategory.Rested:
                    factor = 1.0f;
                    break;
                case RestCategory.Tired:
                    factor = 0.7f;
                    break;
                case RestCategory.VeryTired:
                    factor = 0.3f;
                    break;
                case RestCategory.Exhausted:
                    factor = 0.6f;
                    break;
                default:
                    factor = 999f;
                    break;
            }

            var restGain = 200f / 2.0f * 1.58333332E-05f * factor;

            restNeed.CurLevel += restGain;
        }
    }
}