using DInterests;
using Verse;
using Verse.AI;

namespace DInterestsCallings
{
    internal class JoyGivingDef : InterestDef
    {
        public float visibleJoyFactorForSkill = 0.0020f; // equilibrium at many joy levels

        public override void UpdatePersistentWorkEffect(Pawn pawn, Pawn instigator)
        {
            if (pawn.CanSee(instigator))
            {
                pawn.needs.joy.CurLevel += visibleJoyFactorForSkill;
            }
        }
    }
}