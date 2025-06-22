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
			return Translations_RennOrganisms.Static.SettingsPanelName;
		}


		public override void DoSettingsWindowContents(Rect rect)
		{
            // we will put the rendering code into the settings class - where it belongs...
            ModSettings_RennOrganisms.DoSettingsWindowContents(rect);
		}
	}
}
