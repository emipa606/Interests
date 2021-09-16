using UnityEngine;
using Verse;

namespace DInterestsCallings
{
    internal class InterestsMod : Mod
    {
        private InterestsSettings settings;

        public InterestsMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<InterestsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            InterestsSettings.DrawSettings(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "[D] Interests";
        }


        public override void WriteSettings()
        {
            InterestsSettings.WriteAll();
            base.WriteSettings();
        }
    }
}