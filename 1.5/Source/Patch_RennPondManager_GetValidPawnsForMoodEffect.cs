using HarmonyLib;
using MOARANDROIDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DanielRenner.RennOrganisms
{
    [HarmonyPatch(typeof(GameComponent_RennPondManager), nameof(GameComponent_RennPondManager.GetValidPawnsForMoodEffect))]
    public static class Patch_RennPondManager_GetValidPawnsForMoodEffect
    {
        public static void Postfix(ref IEnumerable<Pawn> __result)
        {
            Log.DebugOnce("Patch_RennOrganisms_AndroidTiers.Postfix for GameComponent_RennPondManager.GetValidPawnsForMoodEffect is getting called..");
            if (__result != null)
            {
                var prevResult = __result;
                __result = prevResult.Where(pawn => { return !Utils.IsBasicAndroidTier(pawn); });
            }
        }
    }
}
