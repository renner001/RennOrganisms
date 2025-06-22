using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DanielRenner.RennOrganisms
{

    [StaticConstructorOnStartup]
    public static class Patch_RennOrganisms_AndroidTiers
    {
        static Patch_RennOrganisms_AndroidTiers()
        {
            Verse.Log.Message("Patch 'Renn Organisms' for 'Android Tiers' loaded");
#if DEBUG
            Harmony.DEBUG = true;
#endif
            Harmony harmony = new Harmony("DanielRenner.RennOrganisms.AndroidTiersPatches");
            harmony.PatchAll();
        }
    }
}