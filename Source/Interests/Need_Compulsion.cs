using System.Collections.Generic;
using DInterests;
using RimWorld;
using UnityEngine;
using Verse;

namespace DInterestsCallings
{
    public class Need_Compulsion : Need
    {
        public enum MoodBuff
        {
            ExtremelyNegative,
            VeryNegative,
            Negative,
            Neutral
        }

        public const int NeuroticTraitDegree = 1;
        public const int VeryNeuroticTraitDegree = 2;

        private const float FallPerTickFactorForNeurotic = 1.2f;
        private const float FallPerTickFactorForVeryNeurotic = 1.3f;

        public const float GainForWork = 0.004f;

        private static readonly SimpleCurve NormalDegreeFallCurve = new SimpleCurve
        {
            new CurvePoint(0f, 0.3f),
            new CurvePoint(NormalDegreeLevelThresholdsForMood.negative, 0.5f),
            new CurvePoint(NormalDegreeLevelThresholdsForMood.negative + 0.001f, 1f),
            new CurvePoint(1f, 1f)
        };

        private static readonly SimpleCurve NeuroticDegreeFallCurve = new SimpleCurve
        {
            new CurvePoint(0f, 0.4f),
            new CurvePoint(NeuroticDegreeLevelThresholdsForMood.negative, 0.6f),
            new CurvePoint(NeuroticDegreeLevelThresholdsForMood.negative + 0.001f, 1f),
            new CurvePoint(1f, 1.10f)
        };

        private static readonly SimpleCurve VeryNeuroticDegreeFallCurve = new SimpleCurve
        {
            new CurvePoint(0f, 0.5f),
            new CurvePoint(VeryNeuroticDegreeLevelThresholdsForMood.negative, 0.7f),
            new CurvePoint(VeryNeuroticDegreeLevelThresholdsForMood.negative + 0.001f, 1f),
            new CurvePoint(1f, 1.20f)
        };

        private static readonly LevelThresholds NormalDegreeLevelThresholdsForMood = new LevelThresholds
        {
            extremelyNegative = 0.01f,
            veryNegative = 0.15f,
            negative = 0.3f
        };

        private static readonly LevelThresholds NeuroticDegreeLevelThresholdsForMood = new LevelThresholds
        {
            extremelyNegative = 0.1f,
            veryNegative = 0.2f,
            negative = 0.4f
        };

        private static readonly LevelThresholds VeryNeuroticDegreeLevelThresholdsForMood = new LevelThresholds
        {
            extremelyNegative = 0.1f,
            veryNegative = 0.3f,
            negative = 0.5f
        };

        public SkillRecord associatedSkill = null;

        public Need_Compulsion(Pawn pawn) : base(pawn)
        {
        }

        private Trait TraitNeurotic
        {
            get
            {
                var neurotic = TraitDef.Named("Neurotic");
                return pawn.story.traits.GetTrait(neurotic);
            }
        }

        public SkillRecord InterestCompulsive
        {
            get
            {
                var compulse = InterestBase.interestList["DCompulsion"];
                if (compulse < 0)
                {
                    return null;
                }

                foreach (var sr in pawn.skills.skills)
                {
                    if ((int) sr.passion == compulse)
                    {
                        return sr;
                    }
                }

                return null;
            }
        }

        private SimpleCurve FallCurve
        {
            get
            {
                var traitNeurotic = TraitNeurotic;
                if (traitNeurotic == null)
                {
                    return NormalDegreeFallCurve;
                }

                if (traitNeurotic.Degree == 1)
                {
                    return NeuroticDegreeFallCurve;
                }

                return VeryNeuroticDegreeFallCurve;
            }
        }

        private float FallPerNeedIntervalTick
        {
            get
            {
                var traitNeurotic = TraitNeurotic;
                var num = 1f;
                if (traitNeurotic != null)
                {
                    num = traitNeurotic.Degree == 1 ? FallPerTickFactorForNeurotic : FallPerTickFactorForVeryNeurotic;
                }

                num *= FallCurve.Evaluate(CurLevel);
                return def.fallPerDay * num / 60000f * 150f;
            }
        }

        private LevelThresholds CurrentLevelThresholds
        {
            get
            {
                var trait = TraitNeurotic;
                if (trait == null)
                {
                    return NormalDegreeLevelThresholdsForMood;
                }

                if (trait.Degree == NeuroticTraitDegree)
                {
                    return NeuroticDegreeLevelThresholdsForMood;
                }

                return VeryNeuroticDegreeLevelThresholdsForMood;
            }
        }

        public MoodBuff MoodBuffForCurrentLevel
        {
            get
            {
                if (Disabled)
                {
                    return MoodBuff.Neutral;
                }

                var currentLevelThresholds = CurrentLevelThresholds;
                var curLevel = CurLevel;
                if (curLevel <= currentLevelThresholds.extremelyNegative)
                {
                    return MoodBuff.ExtremelyNegative;
                }

                if (curLevel <= currentLevelThresholds.veryNegative)
                {
                    return MoodBuff.VeryNegative;
                }

                if (curLevel <= currentLevelThresholds.negative)
                {
                    return MoodBuff.Negative;
                }

                return MoodBuff.Neutral;
            }
        }

        public override int GUIChangeArrow
        {
            get
            {
                var curInstantLevelPercentage = CurInstantLevelPercentage;
                if (curInstantLevelPercentage > CurLevelPercentage + 0.05f)
                {
                    return 1;
                }

                if (curInstantLevelPercentage < CurLevelPercentage - 0.05f)
                {
                    return -1;
                }

                return 0;
            }
        }

        public override bool ShowOnNeedList => !Disabled;

        private bool Disabled => InterestCompulsive == null;

        public void Notify_Worked(float xp)
        {
            CurLevel += xp * GainForWork;
        }

        public override void SetInitialLevel()
        {
            CurLevel = 1f;
        }

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1f,
            bool drawArrows = true, bool doTooltip = true, Rect? rectForTooltip = null)
        {
            var unused = TraitNeurotic;
            var skill = InterestCompulsive;
            if (skill != null && skill != associatedSkill)
            {
                threshPercents = new List<float>();
                var currentLevelThresholds = CurrentLevelThresholds;
                threshPercents.Add(currentLevelThresholds.extremelyNegative);
                threshPercents.Add(currentLevelThresholds.veryNegative);
                threshPercents.Add(currentLevelThresholds.negative);
            }

            base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip, rectForTooltip);
        }

        public override void NeedInterval()
        {
            if (Disabled)
            {
                SetInitialLevel();
                return;
            }

            if (IsFrozen)
            {
                return;
            }

            CurLevel -= FallPerNeedIntervalTick;
        }

        public struct LevelThresholds
        {
            public float extremelyNegative;
            public float veryNegative;
            public float negative;
        }
    }
}