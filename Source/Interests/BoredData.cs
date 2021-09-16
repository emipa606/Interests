using System.Collections.Generic;
using Verse;

namespace DInterestsCallings
{
    internal class BoredData : MapComponent
    {
        private List<Pawn> boredomKeys = new List<Pawn>();
        public Dictionary<Pawn, float> boredomLevels = new Dictionary<Pawn, float>();
        private List<float> boredomVals = new List<float>();
        private List<Pawn> napKeys = new List<Pawn>();
        public Dictionary<Pawn, int> napTimers = new Dictionary<Pawn, int>();
        private List<int> napValues = new List<int>();

        public BoredData(Map map) : base(map)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref boredomLevels, "BoredomLevels", LookMode.Reference, LookMode.Value,
                ref boredomKeys, ref boredomVals);
            Scribe_Collections.Look(ref napTimers, "NapTimers", LookMode.Reference, LookMode.Value, ref napKeys,
                ref napValues);
        }
    }
}