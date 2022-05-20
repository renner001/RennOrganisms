using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace DanielRenner.RennOrganisms
{
    [StaticConstructorOnStartup]
    public class Mod_RennOrganisms : Mod
    {
        static Mod_RennOrganisms()
        {
            Verse.Log.Message("Mod 'Renn Organisms': loaded");
#if DEBUG
            Harmony.DEBUG = true;
#endif
            Harmony harmony = new Harmony("DanielRenner.RennOrganisms");
            harmony.PatchAll();
        }

        public Mod_RennOrganisms(ModContentPack mcp) : base(mcp)
        {
            LongEventHandler.ExecuteWhenFinished(() => { 
				base.GetSettings<ModSettings_RennOrganisms>(); 
			});
        }

		public override void WriteSettings()
		{
			base.WriteSettings();
		}


		public override string SettingsCategory()
		{
			return Translations_RennOrganisms.SettingsPanelName;
		}


		public override void DoSettingsWindowContents(Rect rect)
		{

			Rect descriptionRect = rect.TopPartPixels(Text.CalcHeight(Translations_RennOrganisms.SettingsPanelChangeSettingsEffect, rect.width));
			Rect mainRect = rect.BottomPartPixels(rect.height - descriptionRect.height - 50);
			Widgets.Label(descriptionRect, Translations_RennOrganisms.SettingsPanelChangeSettingsEffect);


			Rect leftRect = mainRect.LeftHalf().Rounded();
			Rect rightRect = mainRect.RightHalf().Rounded();

			Listing_Standard listLeft = new Listing_Standard()
			{
				ColumnWidth = leftRect.width,
			};

			listLeft.Begin(leftRect);
			//listLeft.CheckboxLabeled(Translations_RennOrganisms.EnableResearchGlobalWorkSpeed, ref ModSettings_RennOrganisms.enabledResearchGlobalWorkSpeed, Translations_RennOrganisms.EnableResearchGlobalWorkSpeedTooltip);
			listLeft.End();
		}
	}
}
