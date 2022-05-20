using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DanielRenner.RennOrganisms
{
    [HarmonyPatch(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultThreatPointsNow), new[] { typeof(IIncidentTarget) })]
    /// <summary>
    /// patches the calculation for threat points for immediate events to use the renn pond mechanics
    /// </summary>
    public static class Patch_StorytellerUtility
    {
        public static void Postfix(ref float __result)
        {
            Log.Debug("Postfix for StorytellerUtility.DefaultThreatPointsNow() called");
            var rennPondManager = Current.Game?.GetComponent<GameComponent_RennPondManager>();
            var threatSettings = rennPondManager.GetCurrentSettings();
            var oldValue = __result;
            var newValueByPercentage = Math.Min(__result * threatSettings.threatMultiplier, threatSettings.threatCap);
            var newValue = newValueByPercentage;
            if (newValueByPercentage - oldValue < threatSettings.minThreatReduction)
            {
                newValue = oldValue - threatSettings.minThreatReduction;
            }
            if (newValue < 50)
            {
                newValue = 50;
            }
            Log.Debug("new threat points based on cap=" + threatSettings.threatCap + ", minReduction=" + threatSettings.minThreatReduction + " and multiplier=" + threatSettings.threatMultiplier + ": now=" + newValue + " from=" + __result);
            __result = newValue;
        }
    }
}
