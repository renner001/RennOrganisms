﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DanielRenner.RennOrganisms
{
    class ModSettings_RennOrganisms : ModSettings
    {
        /*
        public static bool enabledResearchGlobalWorkSpeed = true;

        public override void ExposeData()
        {
            base.ExposeData();

            // all scribes...
            Scribe_Values.Look(ref enabledResearchGlobalWorkSpeed, "enabledResearchGlobalWorkSpeed", true);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Log.Debug("ModSettings_RepeatableResearch.ExposeData() post load init called. Setting up research options");
                if (DefDatabase<ResearchProjectDef>.AllDefs.Contains(DefOfs_RepeatableResearch.RepeatablePermanentGlobalWorkSpeed) && !enabledResearchGlobalWorkSpeed)
                {
                    Log.Debug("removing research RepeatablePermanentGlobalWorkSpeed from research database");
                    RemovePublic(DefOfs_RepeatableResearch.RepeatablePermanentGlobalWorkSpeed);
                }
                else if (!DefDatabase<ResearchProjectDef>.AllDefs.Contains(DefOfs_RepeatableResearch.RepeatablePermanentGlobalWorkSpeed) && enabledResearchGlobalWorkSpeed)
                {
                    Log.Debug("pushing research RepeatablePermanentGlobalWorkSpeed to research database");
                    DefDatabase<ResearchProjectDef>.Add(DefOfs_RepeatableResearch.RepeatablePermanentGlobalWorkSpeed);
                }
                else
                {
                    Log.Debug("no changes for RepeatablePermanentGlobalWorkSpeed required, already " + (enabledResearchGlobalWorkSpeed ? "loaded" : "unloaded"));
                }
            }
                
        }
        */
    }
}
