using System;
using System.Collections.Generic;
using System.Linq;
using DInterests;
using UnityEngine;
using Verse;

namespace DInterestsCallings
{
    internal class InterestsSettings : ModSettings
    {
        public static Vector2 scrollPos;
        public static Dictionary<string, int> learnRates;
        public static Dictionary<string, bool> canAppear;
        public static Dictionary<string, int> defaults;

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref learnRates, "learnRates", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref canAppear, "canAppear", LookMode.Value, LookMode.Value);
            base.ExposeData();
        }

        public static void WriteAll()
        {
            if (defaults == null)
            {
                defaults = new Dictionary<string, int>();
                foreach (var id in InterestBase.interestList)
                {
                    defaults.Add(id.defName, (int) Math.Round(id.learnFactor * 100f));
                }
            }

            foreach (var kvp in learnRates ?? Enumerable.Empty<KeyValuePair<string, int>>())
            {
                var id = InterestBase.interestList[kvp.Key];
                if (id != -1)
                {
                    InterestBase.interestList[id].learnFactor = kvp.Value / 100f;
                }
            }

            foreach (var kvp in canAppear ?? Enumerable.Empty<KeyValuePair<string, bool>>())
            {
                var id = InterestBase.interestList[kvp.Key];
                if (id != -1)
                {
                    InterestBase.interestList[id].canAppear = kvp.Value;
                }
            }
        }

        public static void DrawSettings(Rect rect)
        {
            var ls = new Listing_Standard(GameFont.Small);

            var height = 52 + (InterestBase.interestList.Count * ((Text.LineHeight * 3) + (12 * 1) + 5));
            var contents = new Rect(rect.x, rect.y, rect.width - 30f, height);
            Widgets.BeginScrollView(rect, ref scrollPos, contents);
            ls.ColumnWidth = contents.width * 2.0f / 3.0f;
            ls.Begin(contents);
            ls.Gap();
            ls.Label("Warning: Disabling Minor or Major passion may break some mods.");

            if (learnRates == null)
            {
                learnRates = new Dictionary<string, int>();
            }

            if (canAppear == null)
            {
                canAppear = new Dictionary<string, bool>();
            }

            foreach (var id in InterestBase.interestList)
            {
                ls.GapLine();
                DrawInterest(id, ref contents, ls);
            }

            ls.End();
            Widgets.EndScrollView();
        }

        public static void DrawInterest(InterestDef id, ref Rect root, Listing_Standard ls)
        {
            if (!learnRates.ContainsKey(id.defName))
            {
                learnRates.Add(id.defName, (int) Math.Round(id.learnFactor * 100f));
            }

            if (!canAppear.ContainsKey(id.defName))
            {
                canAppear.Add(id.defName, id.canAppear);
            }

            var learnRate = learnRates[id.defName];
            var appears = canAppear[id.defName];

            var image = id.GetTexture();
            Rect labelRect;
            var rectLine = ls.GetRect(Text.LineHeight);

            if (image != null)
            {
                var imageRect = rectLine.LeftPartPixels(Text.LineHeight);
                labelRect = rectLine.RightPartPixels(rectLine.width - Text.LineHeight - 5);
                GUI.DrawTexture(imageRect, image);
            }
            else
            {
                labelRect = rectLine;
            }

            var buffer = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(labelRect, id.LabelCap);
            Text.Anchor = buffer;

            if (id != InterestBase.interestList.GetDefault())
            {
                ls.CheckboxLabeled("Enabled", ref appears, "Allow this interest to appear on new pawns?");
            }

            var learnLine = ls.GetRect(Text.LineHeight);
            var learnLabel = learnLine.LeftHalf();
            var learnField = learnLine.RightHalf();
            buffer = Text.Anchor;
            var lr = "Learn rate";
            if (defaults.ContainsKey(id.defName))
            {
                lr += " (default = " + defaults[id.defName] + ")";
            }

            lr += ": ";
            Widgets.Label(learnLabel, lr);
            Text.Anchor = buffer;
            var stringBuffer = learnRate.ToString();
            Widgets.TextFieldNumeric(learnField, ref learnRate, ref stringBuffer, 0, 10000);

            learnRates[id.defName] = learnRate;
            canAppear[id.defName] = appears;
        }
    }
}